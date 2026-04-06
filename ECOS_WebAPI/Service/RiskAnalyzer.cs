using ECOS_WebAPI.Models;
using ECOS_WebAPI.Service.Interfaces;

namespace ECOS_WebAPI.Service
{
    public class RiskAnalyzer : IRiskAnalyzer
    {
        public List<string> Analyze(Supplier supplier)
        {
            var risks = new List<string>();

            if (supplier.DefectRate > 5)
                risks.Add("High defect rate detected (>5%)");

            if (supplier.ShippingDays >15)
                risks.Add("Slow shipping time (>15 days)");

            if (supplier.MOQ > 1000)
                risks.Add("High MOQ requirement may block scaling");

            if (!supplier.SupportsBranding)
                risks.Add("No branding support");

            if (supplier.DefectRate > 8 && supplier.ShippingDays > 20)
            {
                risks.Add("Critical supplier risk: High defect + slow shipping");
            }

            return risks;
        }
    }
}
