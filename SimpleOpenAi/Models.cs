using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace SimpleOpenAi;

public class Models
{
    private readonly OpenAiClient _openAi;

    public Models(OpenAiClient openAi)
    {
        _openAi = openAi;
    }

    /// <summary>
    ///     Lists the currently available models
    /// </summary>
    /// <returns>Returns a IEnumerable of model ids.</returns>
    public async Task<IEnumerable<string>> ListAsync(CancellationToken cancellationToken = default)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{_openAi.Base}/models");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _openAi.Key);
        var modelsResponse = await OpenAi.HttpClient.SendAsync(httpRequest, cancellationToken);
        modelsResponse.EnsureSuccessStatusCode();
        var data = JObject.Parse(await modelsResponse.Content.ReadAsStringAsync(cancellationToken))["data"]!;
        return data.Select(m => m["id"]!.Value<string>()!);
    }
}