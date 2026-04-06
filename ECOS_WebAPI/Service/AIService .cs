using ECOS_WebAPI.Models;
using ECOS_WebAPI.Models.AI;
using ECOS_WebAPI.Service.Interfaces;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace ECOS_WebAPI.Service
{
    public class AIService : IAIService
    {
        private readonly HttpClient _http;
        private readonly AppSetting _settings;

        public AIService(HttpClient http, IOptions<AppSetting> settings)
        {
            _http = http;
            _settings = settings.Value;
        }

        public async Task<AIProductResponse> GenerateAsync(AIProductRequest request)
        {
            var prompt = request.UserPrompt;

            var body = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                    new
                {
                    role = "system",
                    content = "You are an e-commerce AI. Generate Shopify product + Meta Ads strategy in JSON format only."
                },
                    new
                {
                    role = "user",
                    content = prompt
                }
                }
            };
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, _settings.OpenRouter.BaseUrl);
            httpRequest.Headers.Add("Authorization", $"Bearer {_settings.OpenRouter.ApiKey}");
            httpRequest.Content = new StringContent(JsonSerializer.Serialize(body),
                Encoding.UTF8, "application/json");

            var response = await _http.SendAsync(httpRequest);
            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<AIProductResponse>(json);
        }
    }
}
