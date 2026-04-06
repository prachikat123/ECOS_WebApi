using ECOS_WebAPI.Models;
using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
namespace ECOS_WebAPI.Service
{
    public class MetaCapiService
    {
        private readonly HttpClient _httpClient;
        private readonly MetaSettings _settings;
        public MetaCapiService(HttpClient httpClient, IOptions<MetaSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task SendEvent(LeadRequest request)
        {
            var url = $"https://graph.facebook.com/{_settings.ApiVersion}/{_settings.PixelId}/events?access_token={_settings.AccessToken}";

            var payload = new
            {
                data = new[]
                {
                    new {
                        event_name = request.EventName.ToString(),
                        event_time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                        action_source = "website",
                        user_data = new {
                            em = request.Email != null ? Hash(request.Email) : null,
                            ph = request.Phone != null ? Hash(request.Phone) : null,

                            fbp = request.Fbp,
                            fbc = request.Fbc
                        }
                    }
                },
                test_event_code = _settings.TestCode
            };
            var json = JsonSerializer.Serialize(payload);

            var response = await _httpClient.PostAsync(url,
            new StringContent(json, Encoding.UTF8, "application/json"));

            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);
        }

        private string Hash(string value)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(value.ToLower()));
            return Convert.ToHexString(bytes).ToLower();
        }
    }
}
