using Newtonsoft.Json;
using SimpleOpenAi.Core;

namespace SimpleOpenAi.AudioEndpoint;

public class AudioTranscriptions
{
    private readonly IOpenAiApiRequestHandler _openAiApiRequestHandler;

    public AudioTranscriptions(IOpenAiApiRequestHandler openAiApiRequestHandler)
    {
        _openAiApiRequestHandler = openAiApiRequestHandler;
    }

    public async Task<Result> CreateAsync(
        Stream file, 
        string fileName, 
        string model = "whisper-1", 
        string? language = null, 
        string? prompt = null, 
        float? temperature = null,
        CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, object?>
        {
            { "model", model },
            { "language", language },
            { "prompt", prompt },
            { "temperature", temperature },
        };

        var files = new Dictionary<string, MultipartRequestFile?>
        {
            { "file", new MultipartRequestFile(file, fileName) }
        };

        var result = await _openAiApiRequestHandler.PostMultipartAsync<Result>(
            "/audio/transcriptions", parameters, files, cancellationToken);

        return result;
    }

    public Task<Result> CreateAsync(
        FileStream file,
        string model = "whisper-1",
        string? language = null,
        string? prompt = null,
        float? temperature = null,
        CancellationToken cancellationToken = default)
    {
        return CreateAsync(file, file.Name, model, language, prompt, temperature, cancellationToken);
    }

    public async Task<Result> CreateAsync(
        byte[] fileBytes,
        string fileName,
        string model = "whisper-1",
        string? language = null,
        string? prompt = null,
        float? temperature = null,
        CancellationToken cancellationToken = default)
    {
        using var file = new MemoryStream(fileBytes);
        return await CreateAsync(file, fileName, model, language, prompt, temperature, cancellationToken);
    }

    public record struct Result
    (
        [property: JsonProperty("text")] string Text
    );
}