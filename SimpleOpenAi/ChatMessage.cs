using Newtonsoft.Json;

namespace SimpleOpenAi;

public record struct ChatMessage(
    [property: JsonProperty("role")] string Role,
    [property: JsonProperty("content")] string? Content,
    [property: JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)] string? Name = null,
    [property: JsonProperty("tool_calls", NullValueHandling = NullValueHandling.Ignore)] IReadOnlyList<ChatCompletion.ToolCall>? ToolCalls = null
);