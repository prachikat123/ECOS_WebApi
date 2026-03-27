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
            _httpClient = new HttpClient();
            _settings = settings.Value;
        }
       

        public async Task<string> SendPromptAsync(string prompt)
        {
            var requestBody = new
            {
                model = "openrouter/free",
             
                messages = new[]
                {
                    new
                    {
                        role="user",
                        content = prompt
                    }
                }
            };


            
            var Content = new StringContent(
                 JsonConvert.SerializeObject(requestBody),
                Encoding.UTF8,
                "application/json"
                );
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");
            _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "http://localhost");
            _httpClient.DefaultRequestHeaders.Add("X-Title", "ECOS Project");

            var response = await _httpClient.PostAsync(_settings.BaseUrl, Content);

            var responseString = await response.Content.ReadAsStringAsync();

            Console.WriteLine(responseString);

            if (!response.IsSuccessStatusCode)
            {
                return $"API Error: {response.StatusCode} - {responseString}";
            }

            dynamic json = JsonConvert.DeserializeObject(responseString);

            if(json?.choices == null)
            {
                return "Error: Invalid API response → " + responseString;
            }

            return json.choices[0]?.message?.content ?? "No content found";


        }
    }
}
