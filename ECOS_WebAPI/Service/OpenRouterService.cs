using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.ClientModel.Primitives;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ECOS_WebAPI.Models;


namespace ECOS_WebAPI.Service
{
    public class OpenRouterService
    {
        private readonly HttpClient _httpClient;
        private readonly OpenRouterSettings _settings;



        public OpenRouterService(HttpClient httpClient, IOptions<OpenRouterSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<string> GetCompletion(string prompt)
        {
            
            var requestBody = new
            {
                model = "openai/gpt-4o-mini",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, _settings.BaseUrl);

            request.Content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            request.Headers.Add("Authorization", $"Bearer {_settings.ApiKey}");
            request.Headers.Add("HTTP-Referer", "http://localhost");
            request.Headers.Add("X-Title", "ECOS Project");


            var response = await _httpClient.SendAsync(request);

            var jsonString = await response.Content.ReadAsStringAsync();

            Console.WriteLine("AI RAW RESPONSE:");
            Console.WriteLine(jsonString);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"OpenRouter API Error: {jsonString}");
            }
            var json = JsonDocument.Parse(jsonString);
            var content = json.RootElement
               .GetProperty("choices")[0]
               .GetProperty("message")
               .GetProperty("content")
               .GetString();

            return content;

        }

    }
}
