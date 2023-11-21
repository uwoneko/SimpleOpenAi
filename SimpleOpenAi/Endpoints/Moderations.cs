using Newtonsoft.Json;
using SimpleOpenAi.ApiHandlers;

namespace SimpleOpenAi.Endpoints;

public class Moderations
{
    private readonly IOpenAiApiRequestHandler _openAiApiRequestHandler;

    public Moderations(IOpenAiApiRequestHandler openAiApiRequestHandler)
    {
        _openAiApiRequestHandler = openAiApiRequestHandler;
    }

    /// <summary>
    /// Classifies if text violates OpenAI's content policy.
    /// https://platform.openai.com/docs/api-reference/moderations/create
    /// </summary>
    public async Task<Result> CreateAsync(
        string input,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, object?>
        {
            { "input", input },
            { "model", model }
        };

        var result = await _openAiApiRequestHandler.SendRequestAsync<Result>(
            HttpMethod.Post, "/moderations", parameters, cancellationToken);

        return result;
    }

    /// <summary>
    /// Classifies if texts violate OpenAI's content policy.
    /// https://platform.openai.com/docs/api-reference/moderations/create
    /// </summary>
    public async Task<Result> CreateAsync(
        IEnumerable<string> input,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, object?>
        {
            { "input", input },
            { "model", model }
        };

        var result = await _openAiApiRequestHandler.SendRequestAsync<Result>(
            HttpMethod.Post, "/moderations", parameters, cancellationToken);

        return result;
    }

    /// <summary>
    /// https://platform.openai.com/docs/api-reference/moderations/object
    /// </summary>
    public record struct Result
    (
        [property: JsonProperty("id")] string Id,
        [property: JsonProperty("model")] string Model,
        [property: JsonProperty("results")] IReadOnlyList<ModerationObject> Results
    );

    /// <summary>
    /// https://platform.openai.com/docs/api-reference/moderations/object#moderations/object-results
    /// </summary>
    public record struct ModerationObject
    (
        [property: JsonProperty("flagged")] bool Flagged,
        [property: JsonProperty("categories")] Dictionary<string, bool> Categories,
        [property: JsonProperty("category_scores")] Dictionary<string, float> CategoryScores
    );
}