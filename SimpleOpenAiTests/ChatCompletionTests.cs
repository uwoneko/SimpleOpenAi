using Moq;
using SimpleOpenAi.Endpoints;
using SimpleOpenAi.Interfaces;

namespace SimpleOpenAiTests;

[TestFixture]
public class ChatCompletionTests
{
    [Test]
    public async Task CreateAsync_ParsesResult()
    {
        // Arrange
        var mockHandler = new Mock<IOpenAiApiRequestHandler>();
        var messages = new List<ChatCompletion.Message>
        {
            new("user", "hi")
        };
        
        var mockedResponse = """
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

        mockHandler.Setup(m => m.SendStringRequest(
                HttpMethod.Post,
                "/chat/completions",
                It.IsAny<string>(),
                default))
            .ReturnsAsync(mockedResponse);

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
}