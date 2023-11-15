using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SimpleOpenAi
{
    public class ChatCompletion
    {
        public struct Message
        {
            [JsonProperty("role")]
            public string Role { get; set; }
            [JsonProperty("content")]
            public string Content { get; set; }
            [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
            public string? Name { get; set; }
            [JsonProperty("tool_calls", NullValueHandling = NullValueHandling.Ignore)]
            public IReadOnlyList<ToolCall>? ToolCalls { get; set; }
        }
        
        public struct ToolCall
        {
            [JsonProperty("id")]
            public string Id { get; set; }
            [JsonProperty("type")]
            public string Type { get; set; }
            [JsonProperty("function")]
            public Function Function { get; set; }
        }
        
        public struct Function
        {
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("arguments")]
            public string Arguments { get; set; }
        }
        
        public struct Choice
        {
            [JsonProperty("finish_reason")]
            public string FinishReason { get; init; }
            [JsonProperty("index")]
            public int Index { get; init; }
            [JsonProperty("message")]
            public Message Message { get; init; }
        }
        
        public struct Usage
        {
            [JsonProperty("completion_tokens")]
            public int CompletionTokens { get; init; }
            [JsonProperty("prompt_tokens")]
            public int PromptTokens { get; init; }
            [JsonProperty("total_tokens")]
            public int TotalTokens { get; init; }
        }
        
        public struct Result
        {
            [JsonProperty("id")]
            public string Id { get; init; }
            [JsonProperty("choices")]
            public IReadOnlyList<Choice> Choices { get; init; }
            [JsonProperty("created")]
            public int Created { get; init; }
            [JsonProperty("model")]
            public string Model { get; init; }
            [JsonProperty("system_fingerprint")]
            public string SystemFingerprint { get; init; }
            [JsonProperty("object")]
            public string Object { get; init; }
            [JsonProperty("usage")]
            public Usage Usage { get; init; }
        }
        
        private readonly IOpenAiApiRequestHandler _openAiApiRequestHandler;

        public ChatCompletion(IOpenAiApiRequestHandler openAiApiRequestHandler)
        {
            _openAiApiRequestHandler = openAiApiRequestHandler;
        }

        public async Task<Result> CreateAsync(IEnumerable<Message> messages, string model = "gpt-3.5-turbo")
        {
            var requestBody = new Dictionary<string, object>
            {
                { "model", model },
                { "messages", messages }
            };

            var requestJson = JsonConvert.SerializeObject(requestBody);
            var response = await _openAiApiRequestHandler.SendStringRequest(HttpMethod.Post, "/chat/completions", requestJson);
            
            var result = JsonConvert.DeserializeObject<Result>(response);
            return result;
        }
    }
}