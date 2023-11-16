using SimpleOpenAi.ApiHandlers;
using SimpleOpenAi;

namespace SimpleOpenAi;

public class OpenAi
{
    private OpenAiApiRequestHandler _apiRequestHandler;
    private OpenAiApiKeyProvider _apiKeyProvider;

    public ChatCompletion Chat;

    public OpenAi(string? apiKey = null, string? apiBase = null)
    {
        _apiKeyProvider = new OpenAiApiKeyProvider(apiKey);
        _apiRequestHandler = new OpenAiApiRequestHandler(_apiKeyProvider, apiBase);
        Chat = new ChatCompletion(_apiRequestHandler);
    }
}