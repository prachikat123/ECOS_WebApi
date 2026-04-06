using ECOS_WebAPI.Models;

namespace ECOS_WebAPI.Service
{
    public class SupplierScoringService
    {
        public decimal CalculateScore(Supplier supplier, SourcingApiRequest request)
        {
            decimal priceScore = Math.Max(
                 0,
                (request.TargetSellingPrice - supplier.UnitPrice)
                / request.TargetSellingPrice * 40);
            decimal moqScore = supplier.MOQ <= request.TargetMOQ ? 20 : 5;
            decimal shippingScore = supplier.ShippingDays <= 7 ? 15 : 5;
            decimal defectScore = (1 - supplier.DefectRate / 100) * 15;
            decimal brandingScore = supplier.SupportsBranding ? 10 : 2;

            return priceScore + moqScore + shippingScore + defectScore + brandingScore;
        }
    }
}
