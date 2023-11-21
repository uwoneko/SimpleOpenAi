using Newtonsoft.Json;
using SimpleOpenAi.ApiHandlers;

namespace SimpleOpenAi.Endpoints;

public class Embeddings
{
    private readonly IOpenAiApiRequestHandler _openAiApiRequestHandler;

    public Embeddings(IOpenAiApiRequestHandler openAiApiRequestHandler)
    {
        _openAiApiRequestHandler = openAiApiRequestHandler;
    }

    /// <summary>
    /// Creates an embedding vector representing the input text.
    /// https://platform.openai.com/docs/api-reference/embeddings/create
    /// </summary>
    public async Task<Result> CreateAsync(
        string input,
        string model = "text-embedding-ada-002",
        string? user = null,
        CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, object?>
        {
            { "input", input },
            { "model", model },
            { "user", user },
            { "encoding_format", "float" }
        };

        var result = await _openAiApiRequestHandler.SendRequestAsync<Result>(
            HttpMethod.Post, "/embeddings", parameters, cancellationToken);

        return result;
    }

    /// <summary>
    /// Creates an embedding vector representing the input texts.
    /// https://platform.openai.com/docs/api-reference/embeddings/create
    /// </summary>
    public async Task<Result> CreateAsync(
        IEnumerable<string> input,
        string model = "text-embedding-ada-002",
        string? user = null,
        CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, object?>
        {
            { "input", input },
            { "model", model },
            { "user", user },
            { "encoding_format", "float" }
        };

        var result = await _openAiApiRequestHandler.SendRequestAsync<Result>(
            HttpMethod.Post, "/embeddings", parameters, cancellationToken);

        return result;
    }

    /// <summary>
    /// https://platform.openai.com/docs/api-reference/embeddings/object
    /// </summary>
    public record struct EmbeddingObject
    (
        [property: JsonProperty("object")] string Object,
        [property: JsonProperty("index")] int Index,
        [property: JsonProperty("embedding")] float[] Embedding
    );
    public record struct Result
    (
        [property: JsonProperty("data")] IReadOnlyList<EmbeddingObject> Data,
        [property: JsonProperty("model")] string Model,
        [property: JsonProperty("object")] string Object,
        [property: JsonProperty("usage")] Usage Usage
    )
    {
        /// <summary>
        ///     Points to the first embedding
        /// </summary>
        public float[] Embedding => Data[0].Embedding;
    }

    public record struct Usage
    (
        [property: JsonProperty("prompt_tokens")] int PromptTokens,
        [property: JsonProperty("total_tokens")] int TotalTokens
    );
}