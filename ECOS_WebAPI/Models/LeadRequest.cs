using ECOS_WebAPI.Enum;

namespace ECOS_WebAPI.Models
{
    public class LeadRequest
    {
        public MetaEventType EventName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Fbp { get; set; }
        public string? Fbc { get; set; }
    }
}
