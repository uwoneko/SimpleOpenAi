using SimpleOpenAi.Interfaces;

namespace SimpleOpenAi.Implementations;

public class OpenAiApiKeyProvider : IOpenAiKeyProvider
{
    public OpenAiApiKeyProvider(string? key = null)
    {
        Key = key ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
    }

    public string Key { get; set; }
}