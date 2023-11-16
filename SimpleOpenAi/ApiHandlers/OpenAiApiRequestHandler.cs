using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;

namespace SimpleOpenAi.ApiHandlers;

public class OpenAiApiRequestHandler : IOpenAiApiRequestHandler
{
    public string ApiBase { get; set; }
    private readonly IOpenAiKeyProvider _openAiKeyProvider;

    private static readonly HttpClient HttpClient = new();

    public OpenAiApiRequestHandler(IOpenAiKeyProvider openAiKeyProvider, string? apiBase = null)
    {
        _openAiKeyProvider = openAiKeyProvider;
        ApiBase = apiBase ?? "https://api.openai.com/v1";
    }

    public async Task<string> SendStringRequestAsync(HttpMethod httpMethod, string endpoint, string? body, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(httpMethod, $"{ApiBase}{endpoint}");
        request.Content = new StringContent(body, Encoding.UTF8, "application/json");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _openAiKeyProvider.Key);

        var response = await HttpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }


    public async IAsyncEnumerable<string> SendStreamRequest(HttpMethod httpMethod, string endpoint, string? body,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(httpMethod, $"{ApiBase}{endpoint}");
        request.Content = new StringContent(body, Encoding.UTF8, "application/json");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _openAiKeyProvider.Key);
        
        var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);
        
        while (await reader.ReadLineAsync() is { } line)
        {
            if (!line.ToLower().StartsWith("data:")) continue;

            var dataString = line[5..].Trim();

            if (dataString == "[DONE]") break;

            yield return dataString;
            
            if(cancellationToken.IsCancellationRequested) break;
        }
    }
}