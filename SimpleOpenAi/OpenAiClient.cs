using SimpleOpenAi.ApiHandlers;
using SimpleOpenAi;
using SimpleOpenAi.Endpoints;

namespace SimpleOpenAi;

public class OpenAiClient
{
    private OpenAiApiRequestHandler _apiRequestHandler;
    private OpenAiApiKeyProvider _apiKeyProvider;

    public string ApiKey
    {
        get => _apiKeyProvider.Key;
        set => _apiKeyProvider.Key = value;
    }
    
    public string ApiBase
    {
        get => _apiRequestHandler.ApiBase;
        set => _apiRequestHandler.ApiBase = value;
    }

    public ChatCompletions Chat;

    public ImageGenerations Images;
    
    public Embeddings Embeddings;

    public OpenAiClient(string? apiKey = null, string? apiBase = null)
    {
        _apiKeyProvider = new OpenAiApiKeyProvider(apiKey);
        _apiRequestHandler = new OpenAiApiRequestHandler(_apiKeyProvider, apiBase);
        Chat = new ChatCompletions(_apiRequestHandler);
        Images = new ImageGenerations(_apiRequestHandler);
        Embeddings = new Embeddings(_apiRequestHandler);
    }
}