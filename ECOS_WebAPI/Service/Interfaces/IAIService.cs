using ECOS_WebAPI.Models.AI;

namespace ECOS_WebAPI.Service.Interfaces
{
    public interface IAIService
    {
        Task<AIProductResponse> GenerateAsync(AIProductRequest request);
    }
}
