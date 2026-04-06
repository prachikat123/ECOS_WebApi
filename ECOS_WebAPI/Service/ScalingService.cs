using ECOS_WebAPI.Service.Interfaces;

namespace ECOS_WebAPI.Service
{
    public class ScalingService : IScalingService
    {
        public object Simulate(decimal profitPerUnit)
        {
            return new
            {
                Orders100 = profitPerUnit * 100,
                Orders500 = profitPerUnit * 500,
                Orders1000 = profitPerUnit * 1000
            };
        }
    }
}
