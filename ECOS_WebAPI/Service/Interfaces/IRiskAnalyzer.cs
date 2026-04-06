using ECOS_WebAPI.Models;

namespace ECOS_WebAPI.Service.Interfaces
{
    public interface IRiskAnalyzer
    {
        List<string> Analyze(Supplier supplier);
    }
}
