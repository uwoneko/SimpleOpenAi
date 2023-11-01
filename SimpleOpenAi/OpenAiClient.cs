namespace SimpleOpenAi;

public class OpenAiClient
{
    public readonly Chat Chat;
    public readonly Completions Completions;
    public readonly Images Images;
    public readonly Models Models;
    public readonly Moderations Moderations;
    public readonly Embeddings Embeddings;

    public OpenAiClient(string? apiBase = null, string? apiKey = null)
    {
        Base = apiBase ?? (Environment.GetEnvironmentVariable("OPENAI_API_BASE") ?? "https://api.openai.com/v1")
            .TrimEnd('/');
        Key = apiKey ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
        Chat = new Chat(this);
        Completions = new Completions(this);
        Images = new Images(this);
        Models = new Models(this);
        Moderations = new Moderations(this);
        Embeddings = new Embeddings(this);
    }

    public string Base { get; set; }
    public string Key { get; set; }
}