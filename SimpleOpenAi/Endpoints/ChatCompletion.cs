using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using SimpleOpenAi.Interfaces;

namespace SimpleOpenAi.Endpoints;

public class ChatCompletion
{
    public record struct Message(
        [property: JsonProperty("role")] string Role,
        [property: JsonProperty("content")] string Content,
        [property: JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)] string? Name = null,
        [property: JsonProperty("tool_calls", NullValueHandling = NullValueHandling.Ignore)] IReadOnlyList<ToolCall>? ToolCalls = null
    );

    public record struct ResponseFormat
    (
        [property: JsonProperty("type")] string Type
    );
    
    public record struct FunctionDeclaration
    (
        [property: JsonProperty("description")] string Description,
        [property: JsonProperty("name")] string Name,
        [property: JsonProperty("parameters")] JSchema Parameters
    );
    
    public record struct ToolDeclaration
    (
        [property: JsonProperty("type")] string Type,
        [property: JsonProperty("function")] FunctionDeclaration FunctionDeclaration
    );
    
    public record struct ToolCall
    (
        [property: JsonProperty("id")] string Id,
        [property: JsonProperty("type")] string Type,
        [property: JsonProperty("function")] FunctionCall Function
    );

    public record struct FunctionCall
    (
        [property: JsonProperty("name")] string Name,
        [property: JsonProperty("arguments")] string Arguments
    );

    public record struct Choice
    (
        [property: JsonProperty("finish_reason")] string FinishReason,
        [property: JsonProperty("index")] int Index,
        [property: JsonProperty("message")] Message Message
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
        
    private readonly IOpenAiApiRequestHandler _openAiApiRequestHandler;

    public ChatCompletion(IOpenAiApiRequestHandler openAiApiRequestHandler)
    {
        _openAiApiRequestHandler = openAiApiRequestHandler;
    }

    public async Task<Result> CreateAsync(IEnumerable<Message> messages, string model = "gpt-3.5-turbo",
        int? maxTokens = null, double? presencePenalty = null, 
        double? frequencyPenalty = null, double? temperature = null, double? topP = null, string? stop = null, 
        string? user = null, Dictionary<int, int>? logitBias = null, 
        ResponseFormat? responseFormat = null, int? seed = null, IEnumerable<ToolDeclaration>? tools = null, 
        string? toolChoice = null)
    {
        var parameters = new Dictionary<string, object?>
        {
            { "model", model },
            { "messages", messages },
            { "max_tokens", maxTokens },
            { "presence_penalty", presencePenalty },
            { "frequency_penalty", frequencyPenalty },
            { "temperature", temperature },
            { "top_p", topP },
            { "stop", stop },
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
    
        var response = await _openAiApiRequestHandler.SendStringRequest(HttpMethod.Post, "/chat/completions", requestJson);
        
        var result = JsonConvert.DeserializeObject<Result>(response);
        return result;
    }
}