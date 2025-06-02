using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class OpenAIAIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenAIAIService(IConfiguration config)
    {
        _httpClient = new HttpClient();
        _apiKey = config["OpenAI:ApiKey"];
    }

    public async Task<string> GetResponseAsync(string message, List<UserProgress> progress)
    {
        var context = string.Join("\n", progress.Select(p => $"{p.Action} - {p.Status}"));
        var prompt = $"User asked: {message}\nUser progress:\n{context}\n\nRespond helpfully:";

        var request = new
        {
            model = "gpt-3.5-turbo",
            messages = new[] {
                new { role = "system", content = "You are a helpful banking assistant." },
                new { role = "user", content = prompt }
            }
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", requestContent);
        var responseString = await response.Content.ReadAsStringAsync();
        dynamic result = JsonSerializer.Deserialize<dynamic>(responseString);

        return result.choices[0].message.content.ToString();
    }
}