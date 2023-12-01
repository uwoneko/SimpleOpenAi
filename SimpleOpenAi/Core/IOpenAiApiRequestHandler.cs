namespace SimpleOpenAi.Core;

public interface IOpenAiApiRequestHandler
{
    Task<T> SendAsync<T>(HttpMethod httpMethod, string endpoint, Dictionary<string, object?>? body,
        CancellationToken cancellationToken = default);
}