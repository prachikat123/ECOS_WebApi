namespace ECOS_WebAPI.Models
{
    public class MetaConfig
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public string PageId { get; set; }
        public string AdAccountId { get; set; }
        public string AccessToken { get; set; }

        public bool IsDefault { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }
}
