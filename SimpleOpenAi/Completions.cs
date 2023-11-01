using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SimpleOpenAi;

public class Completions
{
    private readonly OpenAiClient _openAi;

    public Completions(OpenAiClient openAi)
    {
        _openAi = openAi;
    }

    /// <summary>
    ///     Creates a text completion for the provided prompt and parameters.
    /// </summary>
    /// <param name="model">ID of the model to use.</param>
    /// <param name="prompt">The prompt(s) to generate completions for.</param>
    /// <param name="suffix">The suffix that comes after a completion of inserted text.</param>
    /// <param name="maxTokens">The maximum number of tokens to generate in the completion.</param>
    /// <param name="temperature">What sampling temperature to use, between 0 and 2.</param>
    /// <param name="topP">An alternative to sampling with temperature, called nucleus sampling.</param>
    /// <param name="n">How many completions to generate for each prompt.</param>
    /// <param name="logprobs">Include the log probabilities on the logprobs most likely tokens.</param>
    /// <param name="echo">Echo back the prompt in addition to the completion.</param>
    /// <param name="stop">Up to 4 sequences where the API will stop generating further tokens.</param>
    /// <param name="presencePenalty">Positive values penalize new tokens based on whether they appear in the text so far.</param>
    /// <param name="frequencyPenalty">
    ///     Positive values penalize new tokens based on their existing frequency in the text so
    ///     far.
    /// </param>
    /// <param name="bestOf">Generates best_of completions server-side and returns the "best".</param>
    /// <param name="logitBias">Modify the likelihood of specified tokens appearing in the completion.</param>
    /// <param name="user">A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task with value of <see cref="Result" /></returns>
    public async Task<Result> CreateAsync(string prompt, string model = "text-davinci-003",
        double temperature = 1, int maxTokens = 16, double topP = 1, int n = 1,
        int? logprobs = null, bool echo = false, object? stop = null,
        string? suffix = null, int bestOf = 1,
        double presencePenalty = 0, double frequencyPenalty = 0,
        Dictionary<string, double>? logitBias = null, string? user = null,
        CancellationToken cancellationToken = default)
    {
        var requestBody = new Dictionary<string, object>
        {
            { "prompt", prompt },
            { "model", model },
            { "temperature", temperature },
            { "top_p", topP },
            { "n", n },
            { "presence_penalty", presencePenalty },
            { "frequency_penalty", frequencyPenalty },
            { "max_tokens", maxTokens },
            { "echo", echo }
        };

        if (stop != null) requestBody.Add("stop", stop);
        if (logitBias != null) requestBody.Add("logit_bias", logitBias);
        if (logprobs != null) requestBody.Add("logprobs", logprobs);
        if (user != null) requestBody.Add("user", user);
        if (suffix != null) requestBody.Add("suffix", suffix);

        var json = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_openAi.Base}/completions");
        request.Content = content;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _openAi.Key);

        var response = await OpenAi.HttpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseBody = JObject.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
        return new Result
        {
            Raw = responseBody,
            Text = responseBody["choices"]?[0]?["text"]?.Value<string>(),
            FinishReason = responseBody["choices"]?[0]?["finish_reason"]?.Value<string>()
        };
    }

    /// <summary>
    ///     Streams a text completion for the provided prompt and parameters.
    /// </summary>
    /// <param name="model">ID of the model to use.</param>
    /// <param name="prompt">The prompt(s) to generate completions for.</param>
    /// <param name="suffix">The suffix that comes after a completion of inserted text.</param>
    /// <param name="maxTokens">The maximum number of tokens to generate in the completion.</param>
    /// <param name="temperature">What sampling temperature to use, between 0 and 2.</param>
    /// <param name="topP">An alternative to sampling with temperature, called nucleus sampling.</param>
    /// <param name="n">How many completions to generate for each prompt.</param>
    /// <param name="logprobs">Include the log probabilities on the logprobs most likely tokens.</param>
    /// <param name="echo">Echo back the prompt in addition to the completion.</param>
    /// <param name="stop">Up to 4 sequences where the API will stop generating further tokens.</param>
    /// <param name="presencePenalty">Positive values penalize new tokens based on whether they appear in the text so far.</param>
    /// <param name="frequencyPenalty">
    ///     Positive values penalize new tokens based on their existing frequency in the text so
    ///     far.
    /// </param>
    /// <param name="logitBias">Modify the likelihood of specified tokens appearing in the completion.</param>
    /// <param name="user">A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task with value of <see cref="Result" /></returns>
    public async IAsyncEnumerable<Result> CreateStreaming(string prompt, string model = "text-davinci-003",
        double temperature = 1, int maxTokens = 16, double topP = 1, int n = 1,
        int? logprobs = null, bool echo = false, object? stop = null,
        double presencePenalty = 0, double frequencyPenalty = 0, string? suffix = null,
        Dictionary<string, double>? logitBias = null, string? user = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var requestBody = new Dictionary<string, object>
        {
            { "prompt", prompt },
            { "model", model },
            { "temperature", temperature },
            { "top_p", topP },
            { "n", n },
            { "presence_penalty", presencePenalty },
            { "frequency_penalty", frequencyPenalty },
            { "max_tokens", maxTokens },
            { "stream", true },
            { "echo", echo }
        };

        if (stop != null) requestBody.Add("stop", stop);
        if (logitBias != null) requestBody.Add("logit_bias", logitBias);
        if (logprobs != null) requestBody.Add("logprobs", logprobs);
        if (user != null) requestBody.Add("user", user);
        if (suffix != null) requestBody.Add("suffix", suffix);

        var json = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_openAi.Base}/completions");
        request.Content = content;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _openAi.Key);

        var response =
            await OpenAi.HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var reader = new StreamReader(stream);

        while (await reader.ReadLineAsync(cancellationToken) is { } line)
        {
            if (!line.ToLower().StartsWith("data:")) continue;

            var dataString = line[5..].Trim();

            if (dataString == "[DONE]") break;

            var data = JObject.Parse(dataString);

            yield return new Result
            {
                Raw = data,
                Text = data["choices"]?[0]?["text"]?.ToString(),
                FinishReason = data["choices"]?[0]?["finish_reason"]?.ToString()
            };
        }

        reader.Dispose();
    }

    public struct Result
    {
        public JObject Raw;
        public string? Text;
        public string? FinishReason;
    }
}