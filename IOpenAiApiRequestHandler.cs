namespace SimpleOpenAi;

public interface IOpenAiApiRequestHandler
{
    Task<string> SendStringRequest(HttpMethod httpMethod, string endpoint, string? body);
    Task<Stream> SendStreamRequest(HttpMethod httpMethod, string endpoint, string? body);
}