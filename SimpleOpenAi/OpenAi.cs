namespace SimpleOpenAi;

public class OpenAi
{
    protected internal static readonly HttpClient HttpClient = new();
    
    public static OpenAiClient ClientInstance = new();

    public static Chat Chat => ClientInstance.Chat;
    public static Completions Completions => ClientInstance.Completions;
    public static Images Images => ClientInstance.Images;
    public static Models Models => ClientInstance.Models;
    public static Moderations Moderations => ClientInstance.Moderations;
    public static Embeddings Embeddings => ClientInstance.Embeddings;

    public static string ApiKey
    {
        get => ClientInstance.Key;
        set => ClientInstance.Key = value;
    }
    
    public static string ApiBase
    {
        get => ClientInstance.Base;
        set => ClientInstance.Base = value;
    }
}