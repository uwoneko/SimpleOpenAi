namespace SimpleOpenAi
{
    public class ChatCompletion
    {
        public struct Message
        {
            public string Role { get; set; }
            public string Content { get; set; }
            public string? Name { get; set; }
            public IEnumerable<ToolCall>? ToolCalls { get; set; }
        }
        
        public struct ToolCall
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public Function Function { get; set; }
        }
        
        public struct Function
        {
            public string Name { get; set; }
            public string Arguments { get; set; }
        }
        
        public struct Choice
        {
            public string FinishReason { get; init; }
            public int Index { get; init; }
            public Message Message { get; init; }
        }
        
        public struct Usage
        {
            public int CompletionTokens { get; init; }
            public int PromptTokens { get; init; }
            public int TotalTokens { get; init; }
        }
        
        public struct Result
        {
            public string Id { get; init; }
            public IEnumerable<Choice> Choices { get; init; }
            public int Created { get; init; }
            public string Model { get; init; }
            public string SystemFingerprint { get; init; }
            public string Object { get; init; }
            public Usage Usage { get; init; }
        }
        
        private readonly IOpenAiApi _openAiApi;

        public ChatCompletion(IOpenAiApi openAiApi)
        {
            _openAiApi = openAiApi;
        }
    }
}