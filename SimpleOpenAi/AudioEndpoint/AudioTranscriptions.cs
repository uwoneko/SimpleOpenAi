using Newtonsoft.Json;
using SimpleOpenAi.Core;

namespace SimpleOpenAi.AudioEndpoint;

public class AudioTranscriptions
{
    private readonly IOpenAiApiMultipartRequestHandler _openAiApiMultipartRequestHandler;

    public AudioTranscriptions(IOpenAiApiMultipartRequestHandler openAiApiMultipartRequestHandler)
    {
        _openAiApiMultipartRequestHandler = openAiApiMultipartRequestHandler;
    }

    /// <summary>
    /// Transcribes audio into the input language.
    /// https://platform.openai.com/docs/api-reference/audio/createTranscription
    /// </summary>
    public async Task<JsonResult> CreateAsync(
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
            { "response_format", "json" },
        };

        var files = new Dictionary<string, MultipartRequestFile?>
        {
            { "file", new MultipartRequestFile(file, fileName) }
        };

        var result = await _openAiApiMultipartRequestHandler.PostMultipartAsync<JsonResult>(
            "/audio/transcriptions", parameters, files, cancellationToken);

        return result;
    }

    /// <summary>
    /// Transcribes audio into the input language.
    /// https://platform.openai.com/docs/api-reference/audio/createTranscription
    /// </summary>
    public Task<JsonResult> CreateAsync(
        FileStream file,
        string model = "whisper-1",
        string? language = null,
        string? prompt = null,
        float? temperature = null,
        CancellationToken cancellationToken = default)
    {
        return CreateAsync(file, file.Name, model, language, prompt, temperature, cancellationToken);
    }

    /// <summary>
    /// Transcribes audio into the input language.
    /// https://platform.openai.com/docs/api-reference/audio/createTranscription
    /// </summary>
    public async Task<JsonResult> CreateAsync(
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

    /// <summary>
    /// Transcribes audio into the input language.
    /// https://platform.openai.com/docs/api-reference/audio/createTranscription
    /// </summary>
    public async Task<VerboseJsonResult> CreateVerboseAsync(
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
            { "response_format", "verbose_json" },
        };

        var files = new Dictionary<string, MultipartRequestFile?>
        {
            { "file", new MultipartRequestFile(file, fileName) }
        };

        var result = await _openAiApiMultipartRequestHandler.PostMultipartAsync<VerboseJsonResult>(
            "/audio/transcriptions", parameters, files, cancellationToken);

        return result;
    }

    /// <summary>
    /// Transcribes audio into the input language.
    /// https://platform.openai.com/docs/api-reference/audio/createTranscription
    /// </summary>
    public Task<VerboseJsonResult> CreateVerboseAsync(
        FileStream file,
        string model = "whisper-1",
        string? language = null,
        string? prompt = null,
        float? temperature = null,
        CancellationToken cancellationToken = default)
    {
        return CreateVerboseAsync(file, file.Name, model, language, prompt, temperature, cancellationToken);
    }

    /// <summary>
    /// Transcribes audio into the input language.
    /// https://platform.openai.com/docs/api-reference/audio/createTranscription
    /// </summary>
    public async Task<VerboseJsonResult> CreateVerboseAsync(
        byte[] fileBytes,
        string fileName,
        string model = "whisper-1",
        string? language = null,
        string? prompt = null,
        float? temperature = null,
        CancellationToken cancellationToken = default)
    {
        using var file = new MemoryStream(fileBytes);
        return await CreateVerboseAsync(file, fileName, model, language, prompt, temperature, cancellationToken);
    }

    public record struct JsonResult
    (
        [property: JsonProperty("text")] string Text
    );
    
    public record struct VerboseJsonResult
    (
        [property: JsonProperty("task")] string Task,
        [property: JsonProperty("language")] string Language,
        [property: JsonProperty("duration")] double Duration,
        [property: JsonProperty("text")] string Text,
        [property: JsonProperty("segments")] IReadOnlyList<Segment> Segments
    );

    public record struct Segment
    (
        [property: JsonProperty("id")] int Id,
        [property: JsonProperty("seek")] int Seek,
        [property: JsonProperty("start")] double Start,
        [property: JsonProperty("end")] double End,
        [property: JsonProperty("text")] string Text,
        [property: JsonProperty("tokens")] IReadOnlyList<int> Tokens,
        [property: JsonProperty("temperature")] double Temperature,
        [property: JsonProperty("avg_logprob")] double AvgLogprob,
        [property: JsonProperty("compression_ratio")] double CompressionRatio,
        [property: JsonProperty("no_speech_prob")] double NoSpeechProb
    );

}