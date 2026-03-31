using ECOS_WebAPI.Agents;
using ECOS_WebAPI.Models;
using ECOS_WebAPI.Rules;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Reflection;
using System.Runtime;

using System.Threading.Tasks;


namespace ECOS_WebAPI.Service
{
    public class PipelineService
    {
        private readonly ResearchAgent _researchAgent;
        private readonly EvaluationAgent _evaluationAgent;
        private readonly MarketRuleResolver _marketRuleResolver;
        private readonly LanguageResolver _languageResolver;
        private readonly CurrencyService _currencyService;
        private readonly ShopifyService _shopifyService;
        private readonly HttpClient _httpClient;


        public PipelineService(
            ResearchAgent researchAgent,
            EvaluationAgent evaluationAgent,
            MarketRuleResolver marketRuleResolver,
            LanguageResolver languageResolver,
             CurrencyService currencyService,
             ShopifyService shopifyService,
             HttpClient httpClient)
        {
            _researchAgent = researchAgent;
            _evaluationAgent = evaluationAgent;
            _marketRuleResolver = marketRuleResolver;
            _languageResolver = languageResolver;
            _shopifyService = shopifyService;
            _httpClient = httpClient;
        }
        
        private void ApplyMarketBehavior(EcosSetupInput input, List<Product> products)
        {
            var rule = _marketRuleResolver.Resolve(input.TargetCountry);
            foreach (var product in products)
            {
                switch (rule)
                {
                    case MarketRuleType.Premium:
                        product.EstimatedMargin += 5;
                        product.Price *= 1.1m;
                        break;

                    case MarketRuleType.Strict:
                        product.EstimatedMargin -= 10;
                        break;

                    case MarketRuleType.Budget:
                        product.Price *= 0.9m;
                        product.EstimatedMargin += 2;
                        break;
                    case MarketRuleType.Default:
                    default:
                        product.EstimatedMargin += 0;
                        break;
                }
            }
        }

        private void ValidateSetup(EcosSetupInput input)
        {
            if (!input.ShopifyConnected ||
                !input.MetaConnected ||
                !input.GoogleAdsConnected)
               
            {
                throw new Exception("Required platforms are not connected. Agents cannot start.");
            }

            if (string.IsNullOrEmpty(input.TargetCountry))
            {
                throw new Exception("Target market is required.");
            }

            if (input.MinPrice <= 0 || input.MaxPrice <= 0)
            {
                throw new Exception("Invalid price range.");
            }
        }

        public async Task<object> Run(EcosSetupInput input)
        {
            ValidateSetup(input);
            var request = new ResearchRequest
            {
                Niche = input.Niche,
                WebsiteUrl = input.WebsiteUrl
            };
            return await RunAsync(request,input);
        }
        public async Task<List<string>> RunPipeline(string websiteUrl,EcosSetupInput input)
        {
            // Research(REAL OUTPUT)
            var researchProducts = await _researchAgent.GenerateProductIdeas(websiteUrl);
            var createdProducts = new List<string>();

            var products = researchProducts.Select(p => new Product
            {
                Name = p.Title,
                Price = p.Price,
                EstimatedMargin = input.MinMargin + 10
            }).ToList();

            //  Evaluation (AI check)
            var evaluation = await _evaluationAgent.EvaluateWithAI(products, input);

            //  FILTER BEST PRODUCTS
            var bestProducts = evaluation
                .Where(e => e.TotalScore >= 60)
                .ToList();

      
                

                

                foreach (var item in bestProducts)
                {
                    var research = researchProducts
                        .FirstOrDefault(p => p.Title == item.ProductName);

                    if (research == null)
                        continue;

                    //  CURRENCY CONVERSION 
                    var converted = await _currencyService.ConvertAsync(new CurrencyConvertRequest
                    {
                        Amount = research.Price,
                        FromCurrency = "USD",
                        ToCurrency = input.Currency
                    });

                    //  CREATE SHOPIFY PRODUCT
                    var product = new
                    {
                        product = new
                        {
                            title = research.Title,
                            body_html = research.Description,
                            vendor = "ECOS",
                            product_type = research.Category,
                            variants = new[]
                            {
                                new
                                    {
                                        price = converted.ConvertedAmount.ToString("0.00")
                                    }
                            }
                        }
                    };
                    try
                    {
                        await _shopifyService.CreateProduct(product);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    

                    createdProducts.Add(research.Title);
                }
                return createdProducts;
            }
        


        public async Task<PipelineState> RunAsync(ResearchRequest request,EcosSetupInput input)
        {
            
            var state = new PipelineState
            {
                Niche = request.Niche,
                WebsiteUrl = request.WebsiteUrl,
                CurrentStep = "Research"
            };

            //set language
            string language = _languageResolver.Resolve(input.TargetCountry);
            _researchAgent.SetLanguage(language);
            _evaluationAgent.SetLanguage(language);

            List<Product> products = new List<Product>();

            // case 1 : Website Url
            if (!string.IsNullOrEmpty(request.WebsiteUrl))
            {
                var websiteProducts = await _researchAgent.ExtractFromWebsite(request.WebsiteUrl);
                products.AddRange(websiteProducts.Select(p => new Product
                {
                    Name = p,
                    Price = 50,             
                    EstimatedMargin = 30
                }));
            }

            //  Case 2: Niche
            if (!string.IsNullOrEmpty(request.Niche))
            {
                var nicheProducts = await _researchAgent.GetTrendingProducts(request.Niche);
                products.AddRange(nicheProducts.Select(p => new Product
                {
                    Name = p,
                    Price = 50,
                    EstimatedMargin = 30
                }));
            }

            products = products
           .Where(p => !p.Name.ToLower().Contains("login"))
            .Where(p => !p.Name.ToLower().Contains("password"))
            .Where(p => !p.Name.ToLower().Contains("account"))
            .GroupBy(p => p.Name)
            .Select(g => g.First())
            .Distinct()
            .ToList();

            //  Exclusion Rules
            if (input.ExcludedCategories != null && input.ExcludedCategories.Any())
            {
                products = products
                    .Where(p => !input.ExcludedCategories
                        .Any(ex => p.Name.ToLower().Contains(ex.ToLower())))
                    .ToList();
            }

            state.ResearchOutput = products;

            if (!products.Any() || products.Count <= 1)
            {
                if (!string.IsNullOrEmpty(request.Niche))
                {
                    var nicheProducts = await _researchAgent.GetTrendingProducts(request.Niche);
                    products.AddRange(nicheProducts.Select(p => new Product
                    {
                        Name = p,
                        Price = 50,
                        EstimatedMargin = 30
                    }));
                }
            }

            //Step 2 : Evaluation
            state.CurrentStep = "Evaluation";
            state.EvaluationOutput = await _evaluationAgent.EvaluateWithAI(products,input);

            state.IsApproved = state.EvaluationOutput.Any(p => p.TotalScore >= 50);

            return state;

        }
    
    public void Run(List<Product> products)
        {
            foreach (var product in products)
            {
                var rules = _marketRuleResolver.Evaluate(product);
                product.IsApproved = rules.All(r => r.IsPassed);
            }
        }   
    }
}
