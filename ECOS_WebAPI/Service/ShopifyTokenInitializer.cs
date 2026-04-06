using ECOS_WebAPI.Models;
using System.Text.Json;

namespace ECOS_WebAPI.Service
{
    public class ShopifyTokenInitializer : IHostedService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ShopifyAccessToken _tokenStore;
        private readonly IConfiguration _config;

        public ShopifyTokenInitializer(
            IHttpClientFactory httpClientFactory,
            ShopifyAccessToken tokenStore,
            IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _tokenStore = tokenStore;
            _config = config;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient();

            var url = _config["Shopify:StoreUrl"];

            var parameters = new Dictionary<string, string>
        {
            { "client_id", _config["Shopify:ClientId"] },
            { "client_secret", _config["Shopify:ClientSecret"] },
            { "grant_type", _config["Shopify:GrantType"] }
        };

            var response = await client.PostAsync(
                url + "/admin/oauth/access_token",
                new FormUrlEncodedContent(parameters),
                cancellationToken
            );

            var json = await response.Content.ReadAsStringAsync();

            var tokenData = JsonSerializer.Deserialize<ShopifyTokenResponse>(json);

            _tokenStore.SetToken(
                tokenData.access_token,
                tokenData.scope,
                tokenData.expires_in
            );
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
