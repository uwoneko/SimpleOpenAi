using Newtonsoft.Json;

namespace SimpleOpenAi.ChatEndpoint;

public record struct Message(
    [property: JsonProperty("role")] string Role,
    [property: JsonProperty("content")] string? Content,
    [property: JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)] string? Name = null,
    [property: JsonProperty("tool_calls", NullValueHandling = NullValueHandling.Ignore)] IReadOnlyList<ChatCompletions.ToolCall>? ToolCalls = null
);