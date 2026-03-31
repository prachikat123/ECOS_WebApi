using ECOS_WebAPI.Models;
using ECOS_WebAPI.Service;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
namespace ECOS_WebAPI.Agents
{
    public class EvaluationAgent
    {
        private readonly OpenRouterService _aiService;
        public EvaluationAgent(OpenRouterService aiService)
        {
            _aiService = aiService;
        }

        private string _language = "en";
        public void SetLanguage(string language)
        {
            _language = language;
        }


        public async Task<List<EvaluationModel>> EvaluateWithAI(List<Product> products, EcosSetupInput input)
        {
            if (products == null || !products.Any())
                return new List<EvaluationModel>();

            //List<EvaluationModel> result = new List<EvaluationModel>();

            //foreach (var  product in products)
            //{
            //    int score = 100;

            //    if (product.Price < input.MinPrice || product.Price > input.MaxPrice)
            //    {
            //        score -= 20;
            //    }
            //    if (product.EstimatedMargin < input.MinMargin)
            //    {
            //        score -= 30;
            //    }
            //    if (input.ExcludedCategories != null && input.ExcludedCategories.Any(ex => product.Name.ToLower().Contains(ex.ToLower())))
            //    {
            //        score = 0;
            //    }

            //    result.Add(new EvaluationModel
            //    {
            //        ProductName = product.Name,
            //        TotalScore = score
            //    });
            //}
            //return result;
            
            var prompt = $@"
            Evaluate ONLY the following products:
              
            Products:
            {string.Join(", ", products.Select(p => p.Name))}

            IMPORTANT:
           - Use ONLY these product names
            - Do NOT create new products
            - Do NOT use placeholders like Product A/B

            
            Return STRICT JSON:

            [
              {{
                ""productName"": ""same as input"",
                ""trendScore"": number (1-10),
                ""demandScore"": number (1-10),
                ""competitionScore"": number (1-10),
                ""profitScore"": number (1-10),
                ""shippingScore"": number (1-10),
                ""audienceScore"": number (1-10),
                ""repeatScore"": number (1-10)
              }}
            ]
            All scores must be between 1 and 10 only.
            ";

            var response= await _aiService.GetCompletion(prompt);

            var evaluations = ParseResponse(response);

            if (evaluations == null || !evaluations.Any())
                return new List<EvaluationModel>();

            foreach (var item in evaluations)
            {
                item.TotalScore =
                    (item.TrendScore * 2) +
                    (item.DemandScore * 2) +
                    (item.CompetitionScore * 1) +
                    (item.ProfitScore * 2) +
                    (item.ShippingScore * 1) +
                    (item.AudienceScore * 1) +
                    (item.RepeatScore * 1);

                if(item.TotalScore >= 60)
                {
                    item.Recommendation = "Best";
                }
                else if(item.TotalScore >= 45)
                {
                    item.Recommendation = "Average";
                }
                else
                {
                    item.Recommendation = "Risky";
                }
            }
            return evaluations
                 .OrderByDescending(x => x.TotalScore)
                 .ToList();
        }
        private List<EvaluationModel> ParseResponse(string response)
        {
            try
            {
                var jsonStart = response.IndexOf("[");
                var jsonEnd = response.LastIndexOf("]");

                if (jsonStart >= 0 && jsonEnd > jsonStart)
                {
                    var cleanJson = response.Substring(jsonStart, jsonEnd - jsonStart + 1);

                    var result=  JsonConvert.DeserializeObject<List<EvaluationModel>>(cleanJson)
                           ?? new List<EvaluationModel>();
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Parsing Error: " + ex.Message);
                
            }
            return new List<EvaluationModel>();
        }

    }
}
