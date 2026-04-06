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

            products = products
               .Where(p => !string.IsNullOrWhiteSpace(p.Name))
               .GroupBy(p => p.Name)
               .Select(g => g.First())
               .ToList();

            var prompt = $@"
                You are a strict JSON generator.

                ONLY output valid JSON array.

                NO explanation.
                NO markdown.
                NO text.

                Products:
                {string.Join(", ", products.Select(p => p.Name))}

                Return format:
                [
                  {{
                    ""productName"": ""string"",
                    ""trendScore"": 1-10,
                    ""demandScore"": 1-10,
                    ""competitionScore"": 1-10,
                    ""profitScore"": 1-10,
                    ""shippingScore"": 1-10,
                    ""audienceScore"": 1-10,
                    ""repeatScore"": 1-10
                  }}
                ]
                ";

            var response= await _aiService.GetCompletion(prompt);

            if (string.IsNullOrWhiteSpace(response))
                return CreateFallback(products);

            var evaluations = ParseResponse(response);

            if (evaluations == null || !evaluations.Any())
                return CreateFallback(products);

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
                response = response.Trim();
                var jsonStart = response.IndexOf("[");
                var jsonEnd = response.LastIndexOf("]");

                if (jsonStart >= 0 && jsonEnd > jsonStart)
                {
                    var cleanJson = response.Substring(jsonStart, jsonEnd - jsonStart + 1);

                    var result= JsonConvert.DeserializeObject<List<EvaluationModel>>(cleanJson);

                    return result ?? new List<EvaluationModel>();

                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Parsing Error: " + ex.Message);
                
            }
            return new List<EvaluationModel>();
        }

        private List<EvaluationModel> CreateFallback(List<Product> products)
        {
            return products.Select(p => new EvaluationModel
            {
                ProductName = p.Name,
                TrendScore = 5,
                DemandScore = 5,
                CompetitionScore = 5,
                ProfitScore = 5,
                ShippingScore = 5,
                AudienceScore = 5,
                RepeatScore = 5,
                TotalScore = 50,
                Recommendation = "Average"
            }).ToList();
        }


    }
}
