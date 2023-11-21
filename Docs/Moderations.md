# Moderations

https://platform.openai.com/docs/api-reference/moderations

Differences from OpenAI doc:
None

## Examples:
Single:
```csharp
using SimpleOpenAi;

var result = await OpenAi.Moderations.CreateAsync("kill your self");

var resultJson = JsonConvert.SerializeObject(result.Results[0], Formatting.Indented);
Console.WriteLine(resultJson);
```
Multiple:
```csharp
using SimpleOpenAi;

var result = await OpenAi.Moderations.CreateAsync(new [] {"kill your self", "dont kill your self"});

foreach(var result in result.Results)
{
    var resultJson = JsonConvert.SerializeObject(result, Formatting.Indented);
    Console.WriteLine(resultJson);
}
```