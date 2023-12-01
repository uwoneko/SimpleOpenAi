# Audio transcriptions

https://platform.openai.com/docs/api-reference/audio/createTranscription

Differences from OpenAI doc:
- `response_format` is forced to either `json` or `verbose_json` and is decided by either `CreateAsync` or `CreateVerboseAsync`

## Examples:
File + verbose
```csharp
using SimpleOpenAi;
using Newtonsoft.Json;

using(var file = File.OpenRead("alloy.wav"))
{
    var result = await OpenAi.AudioTranscriptions.CreateVerboseAsync(file);

    Console.WriteLine(JsonConvert.SerializeObject(result));
}
```
Bytes + short
```csharp
using SimpleOpenAi;

var bytes = File.ReadAllBytes("alloy.wav");
var result = await OpenAi.AudioTranscriptions.CreateAsync(bytes);

Console.WriteLine(result.Text);
```