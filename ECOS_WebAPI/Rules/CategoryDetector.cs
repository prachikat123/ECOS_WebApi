namespace ECOS_WebAPI.Rules
{
    public class CategoryDetector
    {
        public string Detect(string productName)
        {
            var name = productName.ToLower();

            if (name.Contains("dumbbell") || name.Contains("kettlebell"))
                return "Strength Training";

            if (name.Contains("yoga") || name.Contains("mat"))
                return "Yoga & Flexibility";

            if (name.Contains("band"))
                return "Resistance Training";

            if (name.Contains("roller"))
                return "Recovery";

            return "Other";
        }
    }
}
