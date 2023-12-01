using System.Net;

namespace SimpleOpenAi.Core;

public class OpenAiRequestException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string Content { get; }

    public OpenAiRequestException(HttpStatusCode statusCode, string content)
        : base($"Status code was not 200 when completing an OpenAI request: {(int)statusCode} ({statusCode.ToString()})\n{content}")
    {
        StatusCode = statusCode;
        Content = content;
    }
}