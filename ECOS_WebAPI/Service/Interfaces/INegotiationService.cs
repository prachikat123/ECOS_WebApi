using ECOS_WebAPI.Models;

namespace ECOS_WebAPI.Service.Interfaces
{
    public interface INegotiationService
    {
        decimal GetNegotiatedPrice(Supplier supplier, int expectedDailyOrders);
    }
}
