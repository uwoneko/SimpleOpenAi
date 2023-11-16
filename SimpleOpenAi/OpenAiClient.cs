using SimpleOpenAi.ApiHandlers;
using SimpleOpenAi;

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

    public ChatCompletion Chat;

    public OpenAiClient(string? apiKey = null, string? apiBase = null)
    {
        _apiKeyProvider = new OpenAiApiKeyProvider(apiKey);
        _apiRequestHandler = new OpenAiApiRequestHandler(_apiKeyProvider, apiBase);
        Chat = new ChatCompletion(_apiRequestHandler);
    }
}