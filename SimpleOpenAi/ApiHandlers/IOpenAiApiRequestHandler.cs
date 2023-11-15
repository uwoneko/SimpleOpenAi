namespace SimpleOpenAi.ApiHandlers;

public interface IOpenAiApiRequestHandler
{
    Task<string> SendStringRequestAsync(HttpMethod httpMethod, string endpoint, string? body,
        CancellationToken cancellationToken = default);
    IAsyncEnumerable<string> SendStreamRequest(HttpMethod httpMethod, string endpoint, string? body, 
        CancellationToken cancellationToken = default);
}