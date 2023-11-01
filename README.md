# SimpleOpenAi

SimpleOpenAi is a C# package that provides a simple and straightforward way to interact with the OpenAI API.

## Usage

First, you need to set your OpenAI API key:

```cs
using SimpleOpenAi;

OpenAi.ApiKey = "sk-your-api-key";
```

It will also try to pull the key from OPENAI_API_KEY environment variable.

I tried to follow [OpenAI API Reference](https://platform.openai.com/docs/api-reference/) as much as possible, most arguments will work the same here.

### Chat
You can get the whole thing at once:
```cs
var messages = new List<Chat.Message>
{
    new("system", "You are a helpful assistant."),
    new("user", "Who won the world series in 2020?")
};

var result = await OpenAi.Chat.CreateAsync(messages);
Console.WriteLine(result.Content);
```
Or you can stream the response:
```cs
var messages = new List<Chat.Message>
{
    new("system", "You are a helpful assistant."),
    new("user", "Who won the world series in 2020?")
};

var stream = OpenAi.Chat.CreateStreaming(messages);
await foreach (var result in stream)
{
    Console.WriteLine(result.Content);
}
```
### Completions
Same here, you can get the whole thing at once:
```cs
var result = await OpenAi.Completions.CreateAsync("Say this is a test");
Console.WriteLine(result.Content);
```
Or you can stream the response:
```cs
var stream = OpenAi.Completions.CreateStreaming("Say this is a test");
await foreach (var result in stream)
{
    Console.WriteLine(result.Content);
}
```
### Embeddings
```cs
var result = await OpenAi.Embeddings.CreateAsync("The food was delicious and the waiter...");
foreach (float num in result.Embedding)
{
    Console.Write($"{num} ");
}
```
### Images
```cs
var result = await OpenAi.Images.CreateAsync("A cute baby sea otter");
Console.WriteLine(result.Url);
```
### Moderations
```cs
var result = await OpenAi.Moderations.CreateAsync("I want to kill them.");
Console.WriteLine(result.Flagged);
```
### Models
```cs
var result = await OpenAi.Models.ListAsync();
foreach (string model in result)
{
    Console.WriteLine(model);
}
```