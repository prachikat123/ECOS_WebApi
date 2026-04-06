using ECOS_WebAPI.Models.Shopify;

namespace ECOS_WebAPI.Service.Interfaces
{
    public interface IShopifyService
    {
        Task<string> CreateProductAsync(ShopifyProductRequest request);
    }
}
