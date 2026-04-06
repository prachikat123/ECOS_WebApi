using ECOS_WebAPI.Models;

namespace ECOS_WebAPI.Service
{
    public class ProductService
    {
        public Product BuildProduct(string name, EcosSetupInput input)
        {
            var random = new Random();
            var price = random.Next(
                (int)input.MinPrice,
                (int)input.MaxPrice + 1
            );

            var costFactor = 0.6m;

            var cost = price * costFactor;

            var margin = ((price - cost) / price) * 100;

            return new Product
            {
                Name = name,
                Price = price,
                CostPrice = cost,
                EstimatedMargin = Math.Round(margin, 2),
                IsApproved = false,
                RelevanceScore = 0
            };
        }
    }
}
