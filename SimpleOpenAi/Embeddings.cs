using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SimpleOpenAi;

public class Embeddings
{
    private readonly OpenAiClient _openAi;

    public Embeddings(OpenAiClient openAi)
    {
        _openAi = openAi;
    }
    
    public async Task<Result> CreateAsync(string input, string model = "text-embedding-ada-002",
        string encodingFormat = "float", string? user = null,
        CancellationToken cancellationToken = default)
    {
        var requestBody = new Dictionary<string, object>
        {
            { "input", input },
            { "model", model },
            { "encoding_format", encodingFormat },
        };

        if (user != null) requestBody.Add("user", user);

        var json = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_openAi.Base}/embeddings");
        request.Content = content;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _openAi.Key);

        var response = await OpenAi.HttpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseBody = JObject.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
        return new Result
        {
            Raw = responseBody,
            Embedding = responseBody["data"]?[0]?["embedding"]?.Value<float[]>(),
        };
    }
    
    public struct Result
    {
        public JObject Raw;
        public float[]? Embedding;
    }
}
