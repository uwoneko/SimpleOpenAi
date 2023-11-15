namespace SimpleOpenAi.Interfaces;

public interface IOpenAiApiRequestHandler
{
    Task<string> SendStringRequestAsync(HttpMethod httpMethod, string endpoint, string? body,
        CancellationToken cancellationToken = default);
    IEnumerable<string> SendStreamRequest(HttpMethod httpMethod, string endpoint, string? body, 
        CancellationToken cancellationToken = default);
}