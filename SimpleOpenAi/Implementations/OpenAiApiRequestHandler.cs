using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using SimpleOpenAi.Endpoints;
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
        var request = new HttpRequestMessage(httpMethod, $"{ApiBase}/{endpoint}");
        request.Content = new StringContent(body);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _openAiKeyProvider.Key);
        
        var response = HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();

        var stream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
        using var reader = new StreamReader(stream);
        
        while (reader.ReadLine() is { } line)
        {
            if (!line.ToLower().StartsWith("data:")) continue;

            var dataString = line.Substring(5).Trim();

            if (dataString == "[DONE]") break;

            yield return dataString;
            
            if(cancellationToken.IsCancellationRequested) break;
        }
    }
}