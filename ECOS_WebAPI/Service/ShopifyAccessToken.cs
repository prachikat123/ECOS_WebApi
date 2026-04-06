namespace ECOS_WebAPI.Service
{
    public class ShopifyAccessToken
    {
    
        public string AccessToken { get; private set; }
        public string Scope { get; private set; }
        public int ExpiresIn { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public void SetToken(string accessToken, string scope, int expiresIn)
        {
            AccessToken = accessToken;
            Scope = scope;
            ExpiresIn = expiresIn;
            CreatedAt = DateTime.UtcNow;
        }

        public bool IsExpired()
        {
            return DateTime.UtcNow > CreatedAt.AddSeconds(ExpiresIn - 30);
        }
    }
}
