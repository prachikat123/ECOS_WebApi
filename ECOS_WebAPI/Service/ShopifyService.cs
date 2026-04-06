using ECOS_WebAPI.Models;
using ECOS_WebAPI.Models.Shopify;
using ECOS_WebAPI.Service.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
namespace ECOS_WebAPI.Service
{
    public class ShopifyService : IShopifyService
    {
        private readonly HttpClient _httpClient;
        private readonly ShopifySettings _settings;
        private readonly ShopifyAccessToken _shopifyAccessToken;

        public ShopifyService(HttpClient httpClient, IOptions<ShopifySettings> settings,
            ShopifyAccessToken shopifyAccessToken)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _shopifyAccessToken = shopifyAccessToken;
        }

        public async Task<string> CreateProductAsync(ShopifyProductRequest request)
        {
            var url = $"{_settings.StoreUrl}/admin/api/{_settings.ApiVersion}/products.json";

            var payload = new
            {
                product = new
                {
                    title = request.Title,
                    body_html = request.Description,
                    vendor = request.Vendor,
                    product_type = request.ProductType,
                    variants = new[]
                    {
                        new
                        {
                            price = request.Price,
                            sku = request.SKU
                        }
                    }
                }
            };
            var token = _shopifyAccessToken.AccessToken;

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
            httpRequest.Headers.Add("X-Shopify-Access-Token", token);

            httpRequest.Content = new StringContent(JsonSerializer.Serialize(payload),
            Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest);
            var result = await response.Content.ReadAsStringAsync();

            var parsed = JsonDocument.Parse(result);

            //  STEP 1: CHECK ERROR FIRST
            if (parsed.RootElement.TryGetProperty("errors", out var errors))
            {
                throw new Exception("Shopify Error: " + errors.ToString());
            }

            //  STEP 2: CHECK PRODUCT EXISTS
            if (!parsed.RootElement.TryGetProperty("product", out var product))
            {
                throw new Exception("Invalid Shopify response: product not found");
            }

            //  STEP 3: SAFE READ ID
            if (!product.TryGetProperty("id", out var idElement))
            {
                throw new Exception("Product ID not found in Shopify response");
            }

            return idElement.GetInt64().ToString();
        }
        //public async Task<string> CreateProduct(object productData)
        //{
        //    var url = $"{_settings.StoreUrl}/admin/api/{_settings.ApiVersion}/products.json";

        //    var json = JsonSerializer.Serialize(productData);

        //    var request = new HttpRequestMessage(HttpMethod.Post, url);
        //    request.Headers.Add("X-Shopify-Access-Token", _settings.AccessToken);
        //    request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        //    var response = await _httpClient.SendAsync(request);
        //    var result= await response.Content.ReadAsStringAsync();

        //    return result;

        //}
    }
}
