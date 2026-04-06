namespace ECOS_WebAPI.Service.Interfaces
{
    public interface IMetaOnboardingService
    {
        Task SaveMetaConfigAsync(string userId, string accessToken);
    }
}
