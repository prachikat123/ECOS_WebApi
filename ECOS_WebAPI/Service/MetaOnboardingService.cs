using ECOS_WebAPI.Data;
using ECOS_WebAPI.Models;
using ECOS_WebAPI.Service.Interfaces;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
namespace ECOS_WebAPI.Service
{
    public class MetaOnboardingService : IMetaOnboardingService
    {
        private readonly HttpClient _http;
        private readonly AppDbContext _db;

        public MetaOnboardingService(HttpClient http, AppDbContext db)
        {
            _http = http;
            _db = db;
        }

        public async Task SaveMetaConfigAsync(string userId, string accessToken)
        {
            // Step 1: Get Pages
            var pageResponse = await _http.GetAsync(
                $"https://graph.facebook.com/v25.0/me/accounts?access_token={accessToken}");

            var PageContent = await pageResponse.Content.ReadAsStringAsync();

            if (!pageResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Meta API Error: {PageContent}");
            }

            using var pageDoc = JsonDocument.Parse(PageContent);
            var pages = pageDoc.RootElement.GetProperty("data");

            var pageId = pages.EnumerateArray()
                    .FirstOrDefault()
                    .GetProperty("id")
                    .GetString();


            //Step 2: Get Ad Accounts
            var adResponse = await _http.GetAsync(
                 $"https://graph.facebook.com/v25.0/me/adaccounts?access_token={accessToken}");

            var adContent = await adResponse.Content.ReadAsStringAsync();

            if (!adResponse.IsSuccessStatusCode)
                throw new Exception($"Meta AdAccount API Error: {adContent}");

            using var adDoc = JsonDocument.Parse(adContent);
            var accounts = adDoc.RootElement.GetProperty("data");

            var adAccountId = accounts.EnumerateArray()
                    .FirstOrDefault()
                    .GetProperty("id")
                    .GetString();

            //act_format
            if (!adAccountId.StartsWith("act_"))
                adAccountId = "act_" + adAccountId;

            //  STEP 3: Save to DB
            var existing = await _db.MetaConfigs
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (existing != null)
            {
                existing.PageId = pageId;
                existing.AdAccountId = adAccountId;
                existing.AccessToken = accessToken;
            }
            else
            {
                var config = new MetaConfig
                {
                    UserId = userId,
                    PageId = pageId,
                    AdAccountId = adAccountId,
                    AccessToken = accessToken,
                    CreatedAt = DateTime.UtcNow
                };

                _db.MetaConfigs.Add(config);
            }

            await _db.SaveChangesAsync();
        }
    }
}
