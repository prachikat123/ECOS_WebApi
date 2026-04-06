using ECOS_WebAPI.Models;
using ECOS_WebAPI.Service;
using ECOS_WebAPI.Service.Interfaces;
using OpenAI.Realtime;

namespace ECOS_WebAPI.Agents
{
    public class SourcingAgent
    {
        private readonly ISupplierProvider _supplierProvider;
        private readonly ILandedCostService _costService;
        private readonly IPricingService _pricingService;
        private readonly IProfitService _profitService;
        private readonly IRiskAnalyzer _riskAnalyzer;
        private readonly ISupplierScorer _scorer;
        private readonly INegotiationService _negotiation;
        private readonly IBudgetService _budgetService;

        //private readonly MultiSourceSupplierProvider _multiProvider;

        public SourcingAgent(
        ISupplierProvider supplierProvider,
        ILandedCostService costService,
        IPricingService pricingService,
        IProfitService profitService,
        IRiskAnalyzer riskAnalyzer,
        ISupplierScorer scorer,
        INegotiationService negotiation,
        IBudgetService budgetService)
        //MultiSourceSupplierProvider multiProvider)
        {
            _supplierProvider = supplierProvider;
            _costService = costService;
            _pricingService = pricingService;
            _profitService = profitService;
            _riskAnalyzer = riskAnalyzer;
            _scorer = scorer;
            _negotiation = negotiation;
            _budgetService = budgetService;
            //_multiProvider = multiProvider;
        }
        public async Task<List<SourcingResult>> RunAsync(
            SupplierSearchRequest request,
            decimal targetSellingPrice,
            int expectedDailyOrders)
        {
            var suppliers = await _supplierProvider.GetSuppliersAsync(request);

            var results = new List<SourcingResult>();

            foreach (var supplier in suppliers)
            {
                    // STEP 1: negotiation
                    var negotiatedPrice =
                       _negotiation.GetNegotiatedPrice(supplier, expectedDailyOrders);

                supplier.UnitPrice = negotiatedPrice;

                    // STEP 2: cost
                    var cost = _costService.Calculate(supplier);

                    // STEP 3: Pricing
                    var pricing = _pricingService.CalculatePrice(cost.TotalLandedCost, targetSellingPrice);

                    // STEP 4: Risk
                    var risks = _riskAnalyzer.Analyze(supplier);

                    // STEP 5: Scoring
                    var score = _scorer.Score(supplier, cost, targetSellingPrice);

                    // STEP 6: Approved
                    var margin = pricing.MarginPercentage;
                    var isApproved = margin > 20 && score > 70;


                var result = new SourcingResult
                {
                    Supplier = supplier,
                    Cost = cost,
                    SuggestedSellingPrice = pricing.SellingPrice,
                    MarginPercentage = pricing.MarginPercentage,
                    RiskFlags = risks,
                    Score = score,
                    IsApproved = isApproved
                };

                
                result.DailyAdBudget = _budgetService.CalculateDailyBudget(result);

                results.Add(result);
            }
            Console.WriteLine($"Suppliers count: {suppliers.Count}");

            var bestSupplier = results
                .Where(x => x.IsApproved)
                .OrderByDescending(x => x.Score)
                .FirstOrDefault()
            ?? results.OrderByDescending(x => x.Score).FirstOrDefault();

            return bestSupplier != null
            ? new List<SourcingResult> { bestSupplier }
            : new List<SourcingResult>();
        }
    }
}



    
