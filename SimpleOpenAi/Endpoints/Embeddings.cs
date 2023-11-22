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
    /// <br/><br/>
    /// Prefer to use <see cref="CreateAsync(string,string,string?,System.Threading.CancellationToken)"/> for most cases.
    /// </summary>
    public async Task<FloatsResult> CreateFloatsAsync(
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

        var result = await _openAiApiRequestHandler.SendRequestAsync<FloatsResult>(
            HttpMethod.Post, "/embeddings", parameters, cancellationToken);

        return result;
    }

    /// <summary>
    /// Creates an embedding vector representing the input texts, using the output encoding format "float"
    /// https://platform.openai.com/docs/api-reference/embeddings/create
    /// <br/><br/>
    /// Prefer to use <see cref="CreateAsync(IEnumerable&lt;string&gt;,string,string?,System.Threading.CancellationToken)"/> for most cases.
    /// </summary>
    public async Task<FloatsResult> CreateFloatsAsync(
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

        var result = await _openAiApiRequestHandler.SendRequestAsync<FloatsResult>(
            HttpMethod.Post, "/embeddings", parameters, cancellationToken);

        return result;
    }

    /// <summary>
    /// Creates an embedding vector representing the input texts.
    /// https://platform.openai.com/docs/api-reference/embeddings/create
    /// </summary>
    public async Task<Base64Result> CreateAsync(
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
            { "encoding_format", "base64" }
        };

        var result = await _openAiApiRequestHandler.SendRequestAsync<Base64Result>(
            HttpMethod.Post, "/embeddings", parameters, cancellationToken);

        return result;
    }

    /// <summary>
    /// Creates an embedding vector representing the input texts.
    /// https://platform.openai.com/docs/api-reference/embeddings/create
    /// </summary>
    public async Task<Base64Result> CreateAsync(
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
            { "encoding_format", "base64" }
        };

        var result = await _openAiApiRequestHandler.SendRequestAsync<Base64Result>(
            HttpMethod.Post, "/embeddings", parameters, cancellationToken);

        return result;
    }

    /// <summary>
    /// https://platform.openai.com/docs/api-reference/embeddings/object
    /// </summary>
    public record struct FloatsEmbeddingObject
    (
        [property: JsonProperty("object")] string Object,
        [property: JsonProperty("index")] int Index,
        [property: JsonProperty("embedding")] float[] Embedding
    );
    public record struct FloatsResult
    (
        [property: JsonProperty("data")] IReadOnlyList<FloatsEmbeddingObject> Data,
        [property: JsonProperty("model")] string Model,
        [property: JsonProperty("object")] string Object,
        [property: JsonProperty("usage")] Usage Usage
    )
    {
        /// <summary>
        ///     Returns the first embedding
        /// </summary>
        public float[] Embedding => Data[0].Embedding;
    }

    /// <summary>
    /// https://platform.openai.com/docs/api-reference/embeddings/object
    /// </summary>
    public record struct Base64EmbeddingObject
    (
        [property: JsonProperty("object")] string Object,
        [property: JsonProperty("index")] int Index,
        [property: JsonProperty("embedding")] string EmbeddingBase64
    )
    {
        /// <summary>
        ///     Returns the first embedding
        /// </summary>
        public float[] Embedding
        {
            get
            {
                var bytes = Convert.FromBase64String(EmbeddingBase64);
                
                var floats = new float[bytes.Length / 4];
                for (var i = 0; i < floats.Length; i++)
                {
                    floats[i] = BitConverter.ToSingle(bytes, i * 4);
                }

                return floats;
            }
        }
    }
    
    public record struct Base64Result
    (
        [property: JsonProperty("data")] IReadOnlyList<Base64EmbeddingObject> Data,
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