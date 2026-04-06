namespace ECOS_WebAPI.Rules
{
    public class NicheKeywordProvider
    {
        public List<string> GetKeywords(string niche)
        {
            niche = niche.ToLower();

            if (niche.Contains("fitness"))
            {
                return new List<string>
                {
                    "dumbbell",
                    "yoga",
                    "band",
                    "kettlebell",
                    "roller",
                    "fitness",
                    "exercise",
                    "gym"
                };
            }
            if (niche.Contains("skincare"))
            {
                return new List<string>
                {
                    "cream",
                    "serum",
                    "face",
                    "skin",
                    "beauty"
                };
            }
            return new List<string>();
        }
    }
}
