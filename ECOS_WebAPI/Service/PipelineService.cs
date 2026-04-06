using ECOS_WebAPI.Agents;
using ECOS_WebAPI.Enum;
using ECOS_WebAPI.Models;
using ECOS_WebAPI.Models.Shopify;
using ECOS_WebAPI.Rules;
using ECOS_WebAPI.Service.Interfaces;
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
        private readonly MetaCapiService _metaService;
        private readonly ProductService _productService;
        private readonly SourcingAgent _sourcingAgent;
        private readonly IAdDecisionService _adDecisionService;
        private readonly IMetaAdsService _metaAdsService;
        private readonly IBudgetService _budgetService;
        private readonly IMetaContextService _metaContextService;


        public PipelineService(
            ResearchAgent researchAgent,
            EvaluationAgent evaluationAgent,
            MarketRuleResolver marketRuleResolver,
            LanguageResolver languageResolver,
             CurrencyService currencyService,
             ShopifyService shopifyService,
             MetaCapiService metaService,
             ProductService productService,
             SourcingAgent sourcingAgent,
            IAdDecisionService adDecisionService,
            IMetaAdsService metaAdsService,
            IBudgetService budgetService,
            IMetaContextService metaContextService,
            HttpClient httpClient)
        {
            _researchAgent = researchAgent;
            _evaluationAgent = evaluationAgent;
            _marketRuleResolver = marketRuleResolver;
            _languageResolver = languageResolver;
            _shopifyService = shopifyService;
            _metaService = metaService;
            _productService = productService;
            _adDecisionService = adDecisionService;
            _sourcingAgent = sourcingAgent;
            _metaAdsService = metaAdsService;
            _budgetService = budgetService;
            _metaContextService = metaContextService;
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
        public async Task<List<string>> RunPipeline(string websiteUrl,
            EcosSetupInput input,
            SourcingApiRequest sourcingRequest,
            LeadRequest request)
        {
            await _metaService.SendEvent(new LeadRequest
            {
                EventName = MetaEventType.ViewContent,
                Email = request?.Email
            });

            // step 1: Research(REAL OUTPUT)
            var researchProducts = await _researchAgent.GenerateProductIdeas(websiteUrl,input);
            var createdProducts = new List<string>();

            var products = researchProducts;

            //  step 2 :Evaluation (AI check)
            var evaluation = await _evaluationAgent.EvaluateWithAI(researchProducts, input);

            //  FILTER BEST PRODUCTS
            var bestProducts = evaluation
                .Where(e => e.TotalScore >= input.MinMargin)
                .ToList();

            //step 3 : Map Scores
            foreach (var product in researchProducts)
            {
                var eval = evaluation
                .FirstOrDefault(e => e.ProductName == product.Name);

                if (eval != null)
                {
                    product.RelevanceScore = eval.TotalScore;

                    product.IsApproved = eval.TotalScore >= input.MinMargin;
                }

                
            }

                if (bestProducts.Any())
            {
                await _metaService.SendEvent(new LeadRequest
                {
                    EventName = MetaEventType.Lead,
                    Email = request?.Email
                });
            }

                //process each best product
            foreach (var item in bestProducts)
            {
                var research = researchProducts
                    .FirstOrDefault(p => p.Name == item.ProductName);

                if (research == null)
                    continue;

                // CALL SOURCING AGENT
                var supplierRequest = new SupplierSearchRequest
                {
                    ProductName = research.Name,
                    Country = sourcingRequest.Country,
                    TargetMOQ = sourcingRequest.TargetMOQ
                };

                var sourcingResults = await _sourcingAgent.RunAsync(
                    supplierRequest,
                    sourcingRequest.TargetSellingPrice,
                    sourcingRequest.ExpectedDailyOrders
                    );

                var bestSupplier = sourcingResults?
                    .OrderByDescending(x => x.Score)
                    .FirstOrDefault();


                if (bestSupplier == null)
                    continue;

                
                //  CURRENCY CONVERSION 

                input.Currency ??= "USD";

                var converted = await _currencyService.ConvertAsync(new CurrencyConvertRequest
                {
                    Amount = bestSupplier.SuggestedSellingPrice,
                    FromCurrency = "USD",
                    ToCurrency = input.Currency
                });


                //  step 5 : CREATE SHOPIFY PRODUCT
                var productRequest = new ShopifyProductRequest
                {
                        Title = research.Name,
                        Description = research.Description,
                        Vendor = "ECOS",
                        ProductType = research.Category,
                        Price = Convert.ToDecimal(converted.ConvertedAmount),

                        Images = new List<string>(),
                        Tags = new List<string>(),
                        IsPublished = true

                };
                try
                {
                    await _shopifyService.CreateProductAsync(productRequest);
                    createdProducts.Add(research.Name);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }

                // STEP 6: Ads Decision
                if (_adDecisionService.ShouldRunAds(bestSupplier))
                {
                    var budget = _budgetService.CalculateDailyBudget(bestSupplier);

                    var meta = await _metaContextService.GetMetaContextAsync(input.UserId);
                    var adRequest = new CreateAdRequest
                    {
                        AdAccountId = meta.AdAccountId,
                        PageId = meta.PageId,
                        CampaignName = $"{research.Name} Campaign",
                        AdSetName = "Auto Audience",
                        AdName = $"{research.Name} Ad",

                        WebsiteUrl = input.WebsiteUrl,
                       
                        DailyBudget = budget,
                        CountryCode = MapCountry(input.TargetCountry),
                        Objective = "OUTCOME_SALES"
                    };
                    await _metaAdsService.CreateFullAdFlow(adRequest);

                    Console.WriteLine($"campaign created for {research.Name}");

                }
            }

            // STEP 7: Final conversion event
            if (createdProducts.Any())
            {
                await _metaService.SendEvent(new LeadRequest
                {
                    EventName = MetaEventType.CompleteRegistration,
                    Email = request?.Email
                });
            }
            return createdProducts;

        }

        private string MapCountry(string country)
        {
            return country?.ToLower() switch
            {
                "india" => "IN",
                "united states" => "US",
                "uk" => "GB",
                _ => "IN"
            };
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

                products.AddRange(
                    websiteProducts.Select(p => _productService.BuildProduct(p, input))
                );
            }

            //  Case 2: Niche
            if (!string.IsNullOrEmpty(request.Niche))
            {
                var nicheProducts = await _researchAgent.GetTrendingProducts(request.Niche);
                products.AddRange(
                    nicheProducts.Select(p => _productService.BuildProduct(p, input))
                );
            }

            products = products
                    .Where(p => !string.IsNullOrWhiteSpace(p.Name))
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
                    products.AddRange(
                        nicheProducts.Select(p => _productService.BuildProduct(p, input))
                    );
                }
            }

            ApplyMarketBehavior(input, products);

            state.ResearchOutput = products;
            //Step 2 : Evaluation
            state.CurrentStep = "Evaluation";
            
            var evaluation = await _evaluationAgent.EvaluateWithAI(products, input);

            foreach (var product in products)
            {
                var eval = evaluation.FirstOrDefault(e => e.ProductName.Trim().ToLower() == product.Name.Trim().ToLower());

                if (eval != null)
                {
                    product.RelevanceScore = eval.TotalScore;
                    product.IsApproved = eval.TotalScore >= input.MinMargin;
                }
            }

            if (evaluation == null || !evaluation.Any())
            {
                evaluation = products.Select(p => new EvaluationModel
                {
                    ProductName = p.Name,
                    TotalScore = 50,
                    Recommendation = "Average"
                }).ToList();
            }
            state.EvaluationOutput = evaluation;
            state.IsApproved = evaluation.Any(p => p.TotalScore >= input.MinMargin);


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
