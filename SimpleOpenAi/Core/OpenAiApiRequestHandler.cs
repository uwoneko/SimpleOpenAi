using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;

namespace SimpleOpenAi.Core;

// i go here, fix one thing and hope to never come back again
public class OpenAiApiRequestHandler : IOpenAiApiMultipartRequestHandler, IOpenAiApiStreamHandler
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

    public async Task<T> SendAsync<T>(HttpMethod httpMethod, string endpoint, Dictionary<string, object?>? body,
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
        // ReSharper disable once MethodSupportsCancellation
        var responseContent = await response.Content.ReadAsStringAsync();
        
        if (response.StatusCode != HttpStatusCode.OK)
            throw new OpenAiRequestException(response.StatusCode, responseContent);

        return JsonConvert.DeserializeObject<T>(responseContent)!;
    }


    public async IAsyncEnumerable<T> SendStreaming<T>(HttpMethod httpMethod, string endpoint, Dictionary<string, object?>? body, 
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

        // ReSharper disable once MethodSupportsCancellation
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
    
    public async Task<T> PostMultipartAsync<T>(string endpoint, Dictionary<string, object?>? body,
        Dictionary<string, MultipartRequestFile?>? files,
        CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();
        var request = new HttpRequestMessage(HttpMethod.Post, $"{ApiBase}{endpoint}");
        if (body != null)
        {
            foreach (var (name, value) in body.Where(p => p.Value != null))
            {
                content.Add(new StringContent(value!.ToString()!), JsonConvert.SerializeObject(name));
            }
        }
        if (files != null)
        {
            foreach (var (name, file) in files.Where(p => p.Value != null))
            {
                content.Add(new StreamContent(file!.Stream), name, file.Name);
            }
        }
        
        request.Content = content;

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _openAiKeyProvider.Key);

        var response = await HttpClient.SendAsync(request, cancellationToken);
        // ReSharper disable once MethodSupportsCancellation
        var responseContent = await response.Content.ReadAsStringAsync();
        
        if (response.StatusCode != HttpStatusCode.OK)
            throw new OpenAiRequestException(response.StatusCode, responseContent);

        return JsonConvert.DeserializeObject<T>(responseContent)!;
    }

    public async Task<string> PostMultipartNonJsonAsync<T>(string endpoint, Dictionary<string, object?>? body,
        Dictionary<string, MultipartRequestFile?>? files,
        CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();
        var request = new HttpRequestMessage(HttpMethod.Post, $"{ApiBase}{endpoint}");
        if (body != null)
        {
            foreach (var (name, value) in body.Where(p => p.Value != null))
            {
                content.Add(new StringContent(value!.ToString()!), JsonConvert.SerializeObject(name));
            }
        }
        if (files != null)
        {
            foreach (var (name, file) in files.Where(p => p.Value != null))
            {
                content.Add(new StreamContent(file!.Stream), name, file.Name);
            }
        }
        
        request.Content = content;

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _openAiKeyProvider.Key);

        var response = await HttpClient.SendAsync(request, cancellationToken);
        // ReSharper disable once MethodSupportsCancellation
        var responseContent = await response.Content.ReadAsStringAsync();
        
        if (response.StatusCode != HttpStatusCode.OK)
            throw new OpenAiRequestException(response.StatusCode, responseContent);

        return responseContent;
    }
}