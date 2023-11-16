using JsonAssertions;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using SimpleOpenAi.ApiHandlers;
using SimpleOpenAi;

namespace SimpleOpenAiTests;

[TestFixture]
public class ChatCompletionTests
{
    private const string DummyResponse = """
                                         {
                                             "id": "chatcmpl-8L70GqlOBAbyjzVqkBC58e1eVKrzy",
                                             "object": "chat.completion",
                                             "created": 1700042500,
                                             "model": "gpt-4-0613",
                                             "choices": [
                                                 {
                                                     "index": 0,
                                                     "message": {
                                                         "role": "assistant",
                                                         "content": "Hello! How can I assist you today?\n"
                                                     },
                                                     "finish_reason": "stop"
                                                 }
                                             ],
                                             "usage": {
                                                 "prompt_tokens": 8,
                                                 "completion_tokens": 9,
                                                 "total_tokens": 17
                                             }
                                         }
                                         """;
    
    [Test]
    public async Task CreateAsync_ParsesResultCorrectly()
    {
        // Arrange
        var mockHandler = new Mock<IOpenAiApiRequestHandler>();
        var messages = new List<ChatMessage>
        {
            new("user", "hi")
        };
        
        var expectedResult = new ChatCompletion.Result
        {
            Id = "chatcmpl-8L70GqlOBAbyjzVqkBC58e1eVKrzy",
            Object = "chat.completion",
            Created = 1700042500,
            Model = "gpt-4-0613",
            Choices = new []
            {
                new ChatCompletion.Choice
                {
                    Index = 0,
                    Message = new ChatMessage
                    {
                        Role = "assistant",
                        Content = "Hello! How can I assist you today?\n"
                    },
                    FinishReason = "stop"
                }
            },
            Usage = new ChatCompletion.Usage
            {
                PromptTokens = 8,
                CompletionTokens = 9,
                TotalTokens = 17
            }
        };

        mockHandler.Setup(m => m.SendStringRequestAsync(
                HttpMethod.Post,
                "/chat/completions",
                It.IsAny<string>(),
                default))
            .ReturnsAsync(DummyResponse);

        var chatCompletion = new ChatCompletion(mockHandler.Object);

        // Act
        var result = await chatCompletion.CreateAsync(messages, model: "gpt-4");

        // Assert
        
        Assert.That(result.Id, Is.EqualTo(expectedResult.Id));
        Assert.That(result.Object, Is.EqualTo(expectedResult.Object));
        Assert.That(result.Created, Is.EqualTo(expectedResult.Created));
        Assert.That(result.Model, Is.EqualTo(expectedResult.Model));
        Assert.That(result.Choices, Is.Not.Null);
        for (var i = 0; i < result.Choices.Count; i++)
        {
            Assert.That(result.Choices[i].Index, Is.EqualTo(expectedResult.Choices[i].Index));
            Assert.That(result.Choices[i].Message, Is.EqualTo(expectedResult.Choices[i].Message));
            Assert.That(result.Choices[i].FinishReason, Is.EqualTo(expectedResult.Choices[i].FinishReason));
        }
        Assert.That(result.Usage, Is.EqualTo(expectedResult.Usage));
        
        mockHandler.VerifyAll();
    }
    
    [Test]
    public async Task CreateAsync_SendsCorrectFields()
    {
        // Arrange
        var mockHandler = new Mock<IOpenAiApiRequestHandler>();
        var messages = new List<ChatMessage>
        {
            new("user", "hi")
        };

        var expectedJson = JObject.FromObject(new
        {
            model = "gpt-3.5-turbo",
            messages,
            max_tokens = 100,
            presence_penalty = 0.5,
            frequency_penalty = 0.5,
            temperature = 0.7,
            top_p = 1.0,
            stop = "stop",
            stream = false,
            user = "me",
            logit_bias = new Dictionary<int, int>
            {
                { 1, 1 }
            },
            response_format = new ChatCompletion.ResponseFormat("json"),
            seed = 42,
            tools = new ChatCompletion.ToolDeclaration[]
            {
                new("function", new ChatCompletion.FunctionDeclaration(
                    "Get the current weather in a given location",
                    "get_current_weather",
                    JSchema.Parse("""
                                  {
                                      "type": "object",
                                      "properties": {
                                          "location": {
                                              "type": "string",
                                              "description": "The city and state, e.g. San Francisco, CA"
                                          },
                                          "unit": {
                                              "type": "string",
                                              "enum": [
                                                  "celsius",
                                                  "fahrenheit"
                                              ]
                                          }
                                      },
                                      "required": [
                                          "location"
                                      ]
                                  }
                                  """)))
            },
            tool_choice = "auto"
        });

        mockHandler.Setup(m => m.SendStringRequestAsync(
                HttpMethod.Post,
                "/chat/completions",
                It.IsAny<string>(),
                default))
            .ReturnsAsync(DummyResponse);
        
        var chatCompletion = new ChatCompletion(mockHandler.Object);

        // Act
        await chatCompletion.CreateAsync(messages,
            maxTokens: 100,
            presencePenalty: 0.5,
            frequencyPenalty: 0.5,
            temperature: 0.7,
            topP: 1.0,
            stop: "stop",
            user: "me",
            logitBias: new Dictionary<int, int>
            {
                { 1, 1 }
            },
            responseFormat: new ChatCompletion.ResponseFormat("json"),
            seed: 42,
            tools: new ChatCompletion.ToolDeclaration[]
            {
                new("function", new ChatCompletion.FunctionDeclaration(
                    "Get the current weather in a given location",
                    "get_current_weather",
                    JSchema.Parse("""
                                 {
                                     "type": "object",
                                     "properties": {
                                         "location": {
                                             "type": "string",
                                             "description": "The city and state, e.g. San Francisco, CA"
                                         },
                                         "unit": {
                                             "type": "string",
                                             "enum": [
                                                 "celsius",
                                                 "fahrenheit"
                                             ]
                                         }
                                     },
                                     "required": [
                                         "location"
                                     ]
                                 }
                                 """)))
            },
            toolChoice: "auto"
        );
        
        mockHandler.VerifyAll();

        // Assert
        var json = JObject.Parse((string)mockHandler.Invocations[0].Arguments[2]);
        Console.WriteLine(json);
        
        AssertJson.AreEquals(json, expectedJson);
    }
    
    [Test]
    public async Task CreateStreaming_ParsesStreamCorrectly()
    {
        // Arrange
        var mockHandler = new Mock<IOpenAiApiRequestHandler>();
        var messages = new List<ChatMessage>
        {
            new("user", "hi")
        };

        var dummyStreamResponses = new List<string>
        {
            @"{""id"": ""chunk1"", ""choices"": [{""index"": 0, ""delta"": {""role"": ""assistant"", ""content"": ""Hello""}}], ""created"": 1700042501, ""model"": ""gpt-4-0613"", ""object"": ""stream.chunk""}",
            @"{""id"": ""chunk2"", ""choices"": [{""index"": 0, ""delta"": {""role"": ""assistant"", ""content"": ""!""}}], ""created"": 1700042502, ""model"": ""gpt-4-0613"", ""object"": ""stream.chunk""}",
        };

        var expectedChunks = new List<ChatCompletion.Chunk>
        {
            new()
            {
                Id = "chunk1",
                Choices = new List<ChatCompletion.StreamChoice>
                {
                    new()
                    {
                        Index = 0,
                        Delta = new ChatMessage("assistant", "Hello")
                    }
                },
                Created = 1700042501,
                Model = "gpt-4-0613",
                Object = "stream.chunk"
            },
            new()
            {
                Id = "chunk2",
                Choices = new List<ChatCompletion.StreamChoice>
                {
                    new()
                    {
                        Index = 0,
                        Delta = new ChatMessage("assistant", "!")
                    }
                },
                Created = 1700042502,
                Model = "gpt-4-0613",
                Object = "stream.chunk"
            }
        };

        async IAsyncEnumerable<string> ResponseMock()
        {
            foreach (var response in dummyStreamResponses)
            {
                yield return response;
            }
        }

        mockHandler.Setup(m => m.SendStreamRequest(
                HttpMethod.Post,
                "/chat/completions",
                It.IsAny<string>(),
                default))
            .Returns(ResponseMock);

        var chatCompletion = new ChatCompletion(mockHandler.Object);

        // Act
        var index = 0;
        await foreach (var chunk in chatCompletion.CreateStreaming(messages, cancellationToken: default))
        {
            // Assert
            var expectedChunk = expectedChunks[index];
            Assert.That(chunk.Id, Is.EqualTo(expectedChunk.Id));
            Assert.That(chunk.Choices, Is.Not.Null);
            Assert.That(chunk.Choices.Count, Is.EqualTo(expectedChunk.Choices.Count));
            for (var i = 0; i < chunk.Choices.Count; i++)
            {
                Assert.That(chunk.Choices[i].Index, Is.EqualTo(expectedChunk.Choices[i].Index));
                Assert.That(chunk.Choices[i].Delta, Is.EqualTo(expectedChunk.Choices[i].Delta));
            }
            Assert.That(chunk.Created, Is.EqualTo(expectedChunk.Created));
            Assert.That(chunk.Model, Is.EqualTo(expectedChunk.Model));
            Assert.That(chunk.Object, Is.EqualTo(expectedChunk.Object));

            index++;
        }

        // Ensure that we have processed all expected chunks
        Assert.That(index, Is.EqualTo(expectedChunks.Count));

        mockHandler.VerifyAll();
}

}