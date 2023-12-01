namespace SimpleOpenAi.Core;

public interface IOpenAiApiRequestHandler
{
    Task<T> SendAsync<T>(HttpMethod httpMethod, string endpoint, Dictionary<string, object?>? body,
        CancellationToken cancellationToken = default);
    IAsyncEnumerable<T> SendStreaming<T>(HttpMethod httpMethod, string endpoint, Dictionary<string, object?>? body, 
        CancellationToken cancellationToken = default);
    Task<T> PostMultipartAsync<T>(string endpoint, Dictionary<string, object?>? body,
        Dictionary<string, MultipartRequestFile?>? files,
        CancellationToken cancellationToken = default);
}