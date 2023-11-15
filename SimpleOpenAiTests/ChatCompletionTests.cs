using JsonAssertions;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using SimpleOpenAi.Endpoints;
using SimpleOpenAi.Interfaces;

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
        var messages = new List<ChatCompletion.Message>
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
                    Message = new ChatCompletion.Message
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
        Assert.Multiple(() =>
        {
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
        });
        
        mockHandler.VerifyAll();
    }
    
    [Test]
    public async Task CreateAsync_SendsCorrectFields()
    {
        // Arrange
        var mockHandler = new Mock<IOpenAiApiRequestHandler>();
        var messages = new List<ChatCompletion.Message>
        {
            new("user", "Let's rock this test!")
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
}