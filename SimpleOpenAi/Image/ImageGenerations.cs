using Newtonsoft.Json;
using SimpleOpenAi.Core;

namespace SimpleOpenAi.Image;

public class ImageGenerations
{
    private readonly IOpenAiApiRequestHandler _openAiApiRequestHandler;

    public ImageGenerations(IOpenAiApiRequestHandler openAiApiRequestHandler)
    {
        _openAiApiRequestHandler = openAiApiRequestHandler;
    }

    /// <summary>
    /// Creates an image given a prompt.
    /// </summary>
    /// <returns>A <see cref="UrlResult"/> object with the url to the generated image.</returns>
    public async Task<UrlResult> CreateUrlAsync(
        string prompt,
        string model = "dall-e-2",
        int? n = null,
        string? quality = null,
        string? size = null,
        string? style = null,
        string? user = null,
        CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, object?>
        {
            { "prompt", prompt },
            { "model", model },
            { "n", n },
            { "quality", quality },
            { "user", user },
            { "size", size },
            { "style", style },
            { "response_format", "url" }
        };

        var result = await _openAiApiRequestHandler.SendAsync<UrlResult>(
            HttpMethod.Post, "/images/generations", parameters, cancellationToken);

        return result;
    }

    /// <summary>
    /// Creates an image given a prompt.
    /// </summary>
    /// <returns>A <see cref="Base64Result"/> object with raw bytes of the generated image.</returns>
    public async Task<Base64Result> CreateBytesAsync(
        string prompt,
        string model = "dall-e-2",
        int? n = null,
        string? quality = null,
        string? size = null,
        string? style = null,
        string? user = null,
        CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, object?>
        {
            { "prompt", prompt },
            { "model", model },
            { "n", n },
            { "quality", quality },
            { "user", user },
            { "size", size },
            { "style", style },
            { "response_format", "b64_json" }
        };

        var result = await _openAiApiRequestHandler.SendAsync<Base64Result>(
            HttpMethod.Post, "/images/generations", parameters, cancellationToken);

        return result;
    }

    public record struct UrlImageObject
    (
        [property: JsonProperty("url")] string Url,
        [property: JsonProperty("revised_prompt")] string RevisedPrompt
    );

    public record struct UrlResult
    (
        [property: JsonProperty("created")] int Created,
        [property: JsonProperty("data")] IReadOnlyList<UrlImageObject> Data
    )
    {
        /// <summary>
        ///     Returns url of the first image
        /// </summary>
        [JsonIgnore]
        public string Url => Data[0].Url;

        /// <summary>
        ///     Returns bytes of image at specified index
        /// </summary>
        public string GetUrl(int index)
        {
            return Data[0].Url;
        }
    }

    public record struct Base64ImageObject
    (
        [property: JsonProperty("b64_json")] string Base64Json,
        [property: JsonProperty("revised_prompt")] string RevisedPrompt
    );

    public record struct Base64Result
    (
        [property: JsonProperty("created")] int Created,
        [property: JsonProperty("data")] IReadOnlyList<Base64ImageObject> Data
    )
    {
        /// <summary>
        ///     Returns bytes of the first image
        /// </summary>
        [JsonIgnore]
        public byte[] Bytes => GetBytes(0);

        /// <summary>
        ///     Returns bytes of image at specified index
        /// </summary>
        public byte[] GetBytes(int index)
        {
            return Convert.FromBase64String(Data[index].Base64Json);
        }
    }
}