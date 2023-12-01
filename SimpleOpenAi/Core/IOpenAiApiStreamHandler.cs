namespace SimpleOpenAi.Core;

public interface IOpenAiApiStreamHandler : IOpenAiApiRequestHandler
{
    IAsyncEnumerable<T> SendStreaming<T>(HttpMethod httpMethod, string endpoint, Dictionary<string, object?>? body, 
        CancellationToken cancellationToken = default);
}