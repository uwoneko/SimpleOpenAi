using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using SimpleOpenAi.ApiHandlers;

namespace SimpleOpenAi.Endpoints;

public class ChatCompletions
{
    public record struct ResponseFormat
    (
        [property: JsonProperty("type")] string Type
    );
    
    public record struct FunctionDeclaration
    (
        [property: JsonProperty("name")] string Name,
        [property: JsonProperty("description")] string Description,
        [property: JsonProperty("parameters")] JSchema Parameters
    );
    
    public record struct ToolDeclaration
    (
        [property: JsonProperty("type")] string Type,
        [property: JsonProperty("function")] FunctionDeclaration FunctionDeclaration
    );
    
    public record struct ToolCall
    (
        [property: JsonProperty("id")] string? Id,
        [property: JsonProperty("type")] string? Type,
        [property: JsonProperty("function")] FunctionCall Function
    );

    public record struct FunctionCall
    (
        [property: JsonProperty("name")] string? Name,
        [property: JsonProperty("arguments")] string Arguments
    );

    public record struct Choice
    (
        [property: JsonProperty("finish_reason")] string FinishReason,
        [property: JsonProperty("index")] int Index,
        [property: JsonProperty("message")] ChatMessage Message
    );

    public record struct StreamChoice
    (
        [property: JsonProperty("finish_reason")] string? FinishReason,
        [property: JsonProperty("index")] int Index,
        [property: JsonProperty("delta")] ChatMessage Delta
    );

    public record struct Usage
    (
        [property: JsonProperty("completion_tokens")] int CompletionTokens,
        [property: JsonProperty("prompt_tokens")] int PromptTokens,
        [property: JsonProperty("total_tokens")] int TotalTokens
    );

    public record struct Result
    (
        [property: JsonProperty("id")] string Id,
        [property: JsonProperty("choices")] IReadOnlyList<Choice> Choices,
        [property: JsonProperty("created")] int Created,
        [property: JsonProperty("model")] string Model,
        [property: JsonProperty("system_fingerprint")] string SystemFingerprint,
        [property: JsonProperty("object")] string Object,
        [property: JsonProperty("usage")] Usage Usage
    );

    public record struct Chunk
    (
        [property: JsonProperty("id")] string Id,
        [property: JsonProperty("choices")] IReadOnlyList<StreamChoice> Choices,
        [property: JsonProperty("created")] int Created,
        [property: JsonProperty("model")] string Model,
        [property: JsonProperty("system_fingerprint")] string SystemFingerprint,
        [property: JsonProperty("object")] string Object
    );
        
    private readonly IOpenAiApiRequestHandler _openAiApiRequestHandler;

    public ChatCompletions(IOpenAiApiRequestHandler openAiApiRequestHandler)
    {
        _openAiApiRequestHandler = openAiApiRequestHandler;
    }

    public async Task<Result> CreateAsync(
        IEnumerable<ChatMessage> messages, 
        string model = "gpt-3.5-turbo",
        int? maxTokens = null, 
        int? n = null,
        double? presencePenalty = null, 
        double? frequencyPenalty = null, 
        double? temperature = null, 
        double? topP = null, 
        string? stop = null, 
        string? user = null, 
        int? seed = null, 
        Dictionary<int, int>? logitBias = null, 
        ResponseFormat? responseFormat = null, 
        IEnumerable<ToolDeclaration>? tools = null, 
        string? toolChoice = null, 
        CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, object?>
        {
            { "model", model },
            { "messages", messages },
            { "max_tokens", maxTokens },
            { "n", n },
            { "presence_penalty", presencePenalty },
            { "frequency_penalty", frequencyPenalty },
            { "temperature", temperature },
            { "top_p", topP },
            { "stop", stop },
            { "stream", false },
            { "user", user },
            { "logit_bias", logitBias },
            { "response_format", responseFormat },
            { "seed", seed },
            { "tools", tools },
            { "tool_choice", toolChoice },
        };

        parameters = parameters.Where(p => p.Value != null)
            .ToDictionary(p => p.Key, p => p.Value);

        var requestJson = JsonConvert.SerializeObject(parameters);
    
        var response = await _openAiApiRequestHandler.SendStringRequestAsync(HttpMethod.Post, "/chat/completions", requestJson, cancellationToken);
        
        var result = JsonConvert.DeserializeObject<Result>(response);
        return result;
    }

    public async IAsyncEnumerable<Chunk> CreateStreaming(
        IEnumerable<ChatMessage> messages, 
        string model = "gpt-3.5-turbo",
        int? maxTokens = null, 
        int? n = null,
        double? presencePenalty = null, 
        double? frequencyPenalty = null, 
        double? temperature = null, 
        double? topP = null, 
        string? stop = null, 
        string? user = null, 
        int? seed = null, 
        Dictionary<int, int>? logitBias = null, 
        ResponseFormat? responseFormat = null, 
        IEnumerable<ToolDeclaration>? tools = null, 
        string? toolChoice = null, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, object?>
        {
            { "model", model },
            { "messages", messages },
            { "max_tokens", maxTokens },
            { "n", n },
            { "presence_penalty", presencePenalty },
            { "frequency_penalty", frequencyPenalty },
            { "temperature", temperature },
            { "top_p", topP },
            { "stop", stop },
            { "stream", true },
            { "user", user },
            { "logit_bias", logitBias },
            { "response_format", responseFormat },
            { "seed", seed },
            { "tools", tools },
            { "tool_choice", toolChoice },
        };

        parameters = parameters.Where(p => p.Value != null)
            .ToDictionary(p => p.Key, p => p.Value);

        var requestJson = JsonConvert.SerializeObject(parameters);
    
        var stream = _openAiApiRequestHandler.SendStreamRequest(HttpMethod.Post, "/chat/completions", requestJson, cancellationToken);
        
        await foreach (var s in stream)
        {
            var chunk = JsonConvert.DeserializeObject<Chunk>(s);
            yield return chunk;
        }
    }
}