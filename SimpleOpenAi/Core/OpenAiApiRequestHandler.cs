using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;

namespace SimpleOpenAi.Core;

public class OpenAiApiRequestHandler : IOpenAiApiRequestHandler
{
    public string ApiBase { get; set; }
    private readonly IOpenAiKeyProvider _openAiKeyProvider;

    private static readonly HttpClient HttpClient;

    public OpenAiApiRequestHandler(IOpenAiKeyProvider openAiKeyProvider, string? apiBase = null)
    {
        _openAiKeyProvider = openAiKeyProvider;
        ApiBase = apiBase ?? "https://api.openai.com/v1";
    }

    static OpenAiApiRequestHandler()
    {
        var httpClientHandler = new HttpClientHandler();
        var proxyUriString = Environment.GetEnvironmentVariable("HTTP_PROXY");

        if (!string.IsNullOrWhiteSpace(proxyUriString))
        {
            var proxyUri = new Uri(proxyUriString);
            httpClientHandler.UseProxy = true;

            if (!string.IsNullOrWhiteSpace(proxyUri.UserInfo))
            {
                var credentials = proxyUri.UserInfo.Split(':');
                if (credentials.Length == 2)
                {
                    httpClientHandler.Proxy = new WebProxy(proxyUri.Authority)
                    {
                        Credentials = new NetworkCredential(credentials[0], credentials[1])
                    };
                }
            }
            else
            {
                httpClientHandler.Proxy = new WebProxy(proxyUri);
            }
        }

        HttpClient = new HttpClient(httpClientHandler, disposeHandler: true);
    }

    public async Task<T> SendRequestAsync<T>(HttpMethod httpMethod, string endpoint, Dictionary<string, object?>? body,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(httpMethod, $"{ApiBase}{endpoint}");
        if (body != null)
        {
            var requestBody = new Dictionary<string, object?>(
                body.Where(p => p.Value != null));
            
            request.Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        }
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _openAiKeyProvider.Key);

        var response = await HttpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync())!;
    }


    public async IAsyncEnumerable<T> SendRequestStreaming<T>(HttpMethod httpMethod, string endpoint, Dictionary<string, object?>? body, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(httpMethod, $"{ApiBase}{endpoint}");
        if (body != null)
        {
            var requestBody = new Dictionary<string, object?>(
                body.Where(p => p.Value != null));
            
            request.Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        }
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

            yield return JsonConvert.DeserializeObject<T>(dataString)!;
            
            if(cancellationToken.IsCancellationRequested) break;
        }
    }
}