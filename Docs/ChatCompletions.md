# Chat completions

Provides an implementation of OpenAI API's chat completions.
https://platform.openai.com/docs/api-reference/chat/create

Changes:
- "stream" parameter is split into `CreateStreaming` and `CreateAsync`
- All properties are in `PascalCase`
- All arguments are in `camelCase`
- `tool_choice` can only be a string

## Examples:
Streaming:
```csharp
using SimpleOpenAi;

var messages = new ChatMessage[]
{
    new("user", "hi")
};

var stream = OpenAi.Chat.CreateStreaming(messages, model: "gpt-4-1106-preview");

await foreach (var chunk in stream)
{
    Console.Write(chunk.Choices[0].Delta.Content);
}
```
Non-streaming:
```csharp
using SimpleOpenAi;

var messages = new ChatMessage[]
{
    new("user", "hi")
};

var result = await OpenAi.Chat.CreateAsync(messages, model: "gpt-4-1106-preview");

Console.Write(result.Choices[0].Message.Content);
```
Function calls (non-streaming, but CreateStreaming supports it too):
```csharp
using SimpleOpenAi;
using SimpleOpenAi.Endpoints;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json;

var messages = new ChatMessage[]
{
    new("user", "whats the weather in la")
};

var tools = new ChatCompletions.ToolDeclaration[]
{
    new(Type: "function", FunctionDeclaration: 
        new("get_current_weather",
            "Get the current weather in a given location",
            JSchema.Parse("""
            {
               "type": "object",
               "properties": {
                   "location": {
                       "type": "string",
                       "description": "The city and state, e.g. San Francisco, CA"
                   },
                   "unit": {
                       "type": "string",
                       "enum": [
                       "celsius",
                       "fahrenheit"
                       ]
                   }
               },
               "required": [
               "location"
               ]
           }
           """)))
};

var result = await OpenAi.Chat.CreateAsync(messages, 
    model: "gpt-4-1106-preview", tools: tools);

Console.WriteLine(
    JsonConvert.SerializeObject(
    result.Choices[0].Message, Formatting.Indented));
```