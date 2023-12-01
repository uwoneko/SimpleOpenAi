using SimpleOpenAi.AudioEndpoint;
using SimpleOpenAi.ChatEndpoint;
using SimpleOpenAi.Core;
using SimpleOpenAi.EmbeddingsEndpoint;
using SimpleOpenAi.ImagesEndpoint;
using SimpleOpenAi.ModerationsEndpoint;

namespace SimpleOpenAi;

public class OpenAiClient
{
    private readonly OpenAiApiRequestHandler _apiRequestHandler;
    private readonly OpenAiApiKeyProvider _apiKeyProvider;

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

    public readonly ChatCompletions Chat;

    public readonly ImageGenerations Images;
    
    public readonly Embeddings Embeddings;
    
    public readonly Moderations Moderations;
    
    public readonly AudioTranscriptions AudioTranscriptions;

    public OpenAiClient(string? apiKey = null, string? apiBase = null)
    {
        _apiKeyProvider = new OpenAiApiKeyProvider(apiKey);
        _apiRequestHandler = new OpenAiApiRequestHandler(_apiKeyProvider, apiBase);
        Chat = new ChatCompletions(_apiRequestHandler);
        Images = new ImageGenerations(_apiRequestHandler);
        Embeddings = new Embeddings(_apiRequestHandler);
        Moderations = new Moderations(_apiRequestHandler);
        AudioTranscriptions = new AudioTranscriptions(_apiRequestHandler);
    }
}