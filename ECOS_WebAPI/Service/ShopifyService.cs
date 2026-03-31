using ECOS_WebAPI.Models;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace ECOS_WebAPI.Service
{
    public class ShopifyService
    {
        private readonly HttpClient _httpClient;
        private readonly ShopifySettings _settings;

        public ShopifyService(HttpClient httpClient, IOptions<ShopifySettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }
        public async Task<string> CreateProduct(object productData)
        {
            var url = $"{_settings.StoreUrl}/admin/api/{_settings.ApiVersion}/products.json";

            var json = JsonSerializer.Serialize(productData);

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("X-Shopify-Access-Token", _settings.AccessToken);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var result= await response.Content.ReadAsStringAsync();

            return result;

        }
    }
}
