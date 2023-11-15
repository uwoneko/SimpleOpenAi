using System.Net.Http.Headers;
using SimpleOpenAi.Interfaces;

namespace SimpleOpenAi.Implementations;

public class OpenAiApiRequestHandler : IOpenAiApiRequestHandler
{
    private string ApiBase { get; set; }
    private readonly IOpenAiKeyProvider _openAiKeyProvider;

    private static readonly HttpClient HttpClient = new();

    public OpenAiApiRequestHandler(IOpenAiKeyProvider openAiKeyProvider, string apiBase = "https://api.openai.com/v1")
    {
        _openAiKeyProvider = openAiKeyProvider;
        ApiBase = apiBase;
    }

    public async Task<string> SendStringRequestAsync(HttpMethod httpMethod, string endpoint, string? body, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(httpMethod, $"{ApiBase}/{endpoint}");
        request.Content = new StringContent(body);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _openAiKeyProvider.Key);

        var response = await HttpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }


    public IEnumerable<string> SendStreamRequest(HttpMethod httpMethod, string endpoint, string? body, CancellationToken cancellationToken = default)
    {
        yield return "";
        throw new NotImplementedException();
    }
}