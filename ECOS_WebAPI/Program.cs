

using ECOS_WebAPI.Agents;
using ECOS_WebAPI.Data;
using ECOS_WebAPI.Models;
using ECOS_WebAPI.Rules;
using ECOS_WebAPI.Rules.Interfaces;
using ECOS_WebAPI.Service;
using ECOS_WebAPI.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ECOS_WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.Configure<OpenRouterSettings>(
            builder.Configuration.GetSection("OpenRouter")
            );

            builder.Services.Configure<ShopifySettings>(
            builder.Configuration.GetSection("Shopify")
            );

            builder.Services.Configure<MetaSettings>(
            builder.Configuration.GetSection("Meta")
            );

            builder.Services.Configure<SourcingSettings>(
            builder.Configuration.GetSection("SourcingSettings")
            );

            builder.Services.Configure<ShopifySettings>(
            builder.Configuration.GetSection("Shopify"));

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            builder.Services.AddHttpClient<OpenRouterService>();
            builder.Services.AddHttpClient<ResearchAgent>();
            builder.Services.AddScoped<EvaluationAgent>();
            builder.Services.AddScoped<PipelineService>();
            builder.Services.AddScoped<MarketRuleResolver>();
            builder.Services.AddScoped<LanguageResolver>();
            builder.Services.AddHttpClient<IExchangeRateProvider, ExchangeRateProvider>();
            builder.Services.AddScoped<CurrencyService>();
            builder.Services.AddHttpClient<ShopifyService>();
            builder.Services.AddHttpClient<MetaCapiService>();
            builder.Services.AddScoped<ProductService>();
            builder.Services.AddSingleton<NicheKeywordProvider>();
            builder.Services.AddScoped<ProductRelevanceRule>();
            builder.Services.AddScoped<AIRelevanceService>();
            builder.Services.AddSingleton<CategoryDetector>();
            builder.Services.AddScoped<EmbeddingService>();
            builder.Services.AddHttpClient<MetaAdsService>();

            //sourcing Agent
            // Supplier Providers
            builder.Services.AddScoped<ISupplierProvider, AlibabaSupplierProvider>();

            // Aggregator
            builder.Services.AddScoped<MultiSourceSupplierProvider>();

            // Agent
            builder.Services.AddScoped<SourcingAgent>();

            // Core Services
            builder.Services.AddScoped<ILandedCostService, LandedCostService>();
            builder.Services.AddScoped<IPricingService, PricingService>();
            builder.Services.AddScoped<IProfitService, ProfitService>();
            builder.Services.AddScoped<ISupplierScorer, SupplierScorer>();
            builder.Services.AddScoped<IRiskAnalyzer, RiskAnalyzer>();
            builder.Services.AddScoped<INegotiationService, NegotiationService>();
            builder.Services.AddScoped<ICostCalculator, CostCalculator>();
            builder.Services.AddHttpClient<IMetaOnboardingService, MetaOnboardingService>();
            builder.Services.AddScoped<SourcingAgent>();

            builder.Services.AddScoped<IAdDecisionService, AdDecisionService>();
            builder.Services.AddScoped<IBudgetService, BudgetService>();
            builder.Services.AddHttpClient<IMetaAdsService, MetaAdsService>();
            builder.Services.AddScoped<IMetaContextService, MetaContextService>();

            builder.Services.AddSingleton<ShopifyAccessToken>();
            builder.Services.AddHostedService<ShopifyTokenInitializer>();







            //builder.Services.AddSingleton<OpenRouterService>(sp =>
            //{
            //    var config = sp.GetRequiredService<IConfiguration>();
            //    var apiKey = config["OpenRouter:ApiKey"];

            //    return new OpenRouterService(apiKey);
            //});


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
   
        }
    }
}
