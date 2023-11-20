# Embeddings

https://platform.openai.com/docs/api-reference/chat

Differences from OpenAI doc:
- `encoding_format` parameter is forced to `float`
- Shortcuts for results, `result.Data[0].Embedding` can be replaced with `result.Embedding`


## Examples:
Single:
```csharp
using SimpleOpenAi;
using Newtonsoft.Json;

var result = await OpenAi.Embeddings.CreateAsync(new [] {"Meow!", "A cat"});

Console.WriteLine(JsonConvert.SerializeObject(result.Data.Select(x => x.Embedding)));
```
Multiple:
```csharp
using SimpleOpenAi;
using Newtonsoft.Json;

var result = await OpenAi.Embeddings.CreateAsync("Meow!", "A cat");

Console.WriteLine(JsonConvert.SerializeObject(result.Embedding));
```