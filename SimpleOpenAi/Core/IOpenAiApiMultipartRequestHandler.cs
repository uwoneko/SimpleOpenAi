namespace SimpleOpenAi.Core;

public interface IOpenAiApiMultipartRequestHandler
{
    Task<T> PostMultipartAsync<T>(string endpoint, Dictionary<string, object?>? body,
        Dictionary<string, MultipartRequestFile?>? files,
        CancellationToken cancellationToken = default);
}