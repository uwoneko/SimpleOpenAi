namespace SimpleOpenAi.Core;

public interface IOpenAiApiRequestHandler
{
    Task<T> SendRequestAsync<T>(HttpMethod httpMethod, string endpoint, Dictionary<string, object?>? body,
        CancellationToken cancellationToken = default);
    IAsyncEnumerable<T> SendRequestStreaming<T>(HttpMethod httpMethod, string endpoint, Dictionary<string, object?>? body, 
        CancellationToken cancellationToken = default);
}