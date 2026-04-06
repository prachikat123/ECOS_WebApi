using Azure.Core;
using ECOS_WebAPI.Models;
using ECOS_WebAPI.Service.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using Microsoft.VisualBasic;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace ECOS_WebAPI.Service
{
    public class MetaAdsService : IMetaAdsService
    {
        private readonly HttpClient _http;
        
        private readonly MetaSettings _settings;

        public MetaAdsService(HttpClient http, IOptions<MetaSettings> settings)
        {
            _http = http;
            _settings = settings.Value;
        }

        public async Task<string> UploadImageAsync(string adAccountId, string imageUrl)
        {

            var url = $"{_settings.BaseUrl}{_settings.ApiVersion}/act_{adAccountId}/adimages";

            var data = new Dictionary<string, string>
    {
        { "url", imageUrl },
        { "access_token", _settings.AccessToken }
    };

            var response = await _http.PostAsync(url, new FormUrlEncodedContent(data));
            var json = await response.Content.ReadAsStringAsync();

            Console.WriteLine("UploadImage Response: " + json);


            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Meta Image Upload HTTP Error: {json}");
            }

            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("error", out var error))
            {
                var message = error.GetProperty("message").GetString();
                throw new Exception($"Meta Image Upload Error: {message}");
            }

            var images = doc.RootElement.GetProperty("images");

            foreach (var img in images.EnumerateObject())
            {
                if (img.Value.TryGetProperty("hash", out var hash))
                {
                    return hash.GetString();
                }
            }

            throw new Exception("Image hash not found in response");

        }


        public async Task<string> CreateFullAdFlow(CreateAdRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var campaignId = await CreateCampaign(request);
            var budget = request.DailyBudget; 

            var adSetId = await CreateAdSet(request, campaignId, budget);

            var creativeId = await CreateCreative(request);
            var adId = await CreateAd(request, adSetId, creativeId);

            return $"Campaign: {campaignId}, AdSet: {adSetId}, Creative: {creativeId}, Ad: {adId}";
        }

        // Create Campaign
        private async Task<string> CreateCampaign(CreateAdRequest request)
        {
            var url = $"{_settings.BaseUrl}{_settings.ApiVersion}/{request.AdAccountId}/campaigns";

            var objective = request.Objective switch
            {
                "CONVERSIONS" => "OUTCOME_SALES",
                "WEBSITE_CONVERSIONS" => "OUTCOME_SALES",
                _ => request.Objective ?? "OUTCOME_SALES"
            };

            var payload = new Dictionary<string, string>
            {
                ["name"] = request.CampaignName,
                ["objective"] = objective,
                ["status"] = request.Status ?? "PAUSED",
                ["special_ad_categories"] = "[]",
                ["is_adset_budget_sharing_enabled"] = "false",
                ["access_token"] = _settings.AccessToken
            };

            var response = await _http.PostAsync(url, new FormUrlEncodedContent(payload));

            var json = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Meta Response: " + json);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"HTTP Error: {json}");

            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("error", out var error))
            {
                var fullError = error.ToString();
                Console.WriteLine("FULL META ERROR: " + fullError);
                throw new Exception(fullError);
                
            }

            var campaignId = doc.RootElement.GetProperty("id").GetString();

            return campaignId;

        }

        //  Create Ad Set
        private async Task<string> CreateAdSet(CreateAdRequest request, string campaignId, decimal budget)
        {
            var url = $"{_settings.BaseUrl}{_settings.ApiVersion}/{request.AdAccountId}/adsets";

            var optimizationGoal = request.Objective switch
            {
                "OUTCOME_SALES" => "OFFSITE_CONVERSIONS",
                "OUTCOME_TRAFFIC" => "LINK_CLICKS",
                "OUTCOME_AWARENESS" => "IMPRESSIONS",
                _ => "OFFSITE_CONVERSIONS"
            };
            var payload = new Dictionary<string, string>
            {
                ["name"] = request.AdSetName,
                ["campaign_id"] = campaignId,
                ["daily_budget"] = (request.DailyBudget * 100).ToString(), // cents
                ["billing_event"] = "IMPRESSIONS",
                ["optimization_goal"] = optimizationGoal,
                ["bid_strategy"] = "LOWEST_COST_WITHOUT_CAP",
                ["status"] = "PAUSED",
                ["promoted_object"] = JsonSerializer.Serialize(new
                {
                    pixel_id = _settings.PixelId,
                    custom_event_type = "PURCHASE"
                }),
                ["targeting"] = JsonSerializer.Serialize(new
                {
                    age_min = request.AgeMin,
                    age_max = Math.Max(request.AgeMax, 65),
                    geo_locations = new
                    {
                        countries = new[] { request.CountryCode ?? "IN" }
                    },
                    interests = new object[] { },
                    targeting_automation = new
                    {
                        advantage_audience = 1  
                    }
                }),

                ["access_token"] = _settings.AccessToken
            };

            var response = await _http.PostAsync(url, new FormUrlEncodedContent(payload));
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception(json);

            Console.WriteLine("adset Response: " + json);

            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("error", out var error))
            {
                var fullError = error.ToString();
                Console.WriteLine("FULL META ERROR: " + fullError);
                throw new Exception($"Meta API Error in CreateCampaign: {fullError}");
            }

            var adSetId = doc.RootElement.GetProperty("id").GetString();

            return adSetId;


        }

        //  Create Creative
        private async Task<string> CreateCreative(CreateAdRequest request)
        {
            var url = $"{_settings.BaseUrl}{_settings.ApiVersion}/{request.AdAccountId}/adcreatives";

            var uploadResponse = await UploadImageAsync(request.AdAccountId, request.ImageUrl);

            using var uploadDoc = JsonDocument.Parse(uploadResponse);

            var imageHash = uploadDoc.RootElement
                .GetProperty("images")
                .EnumerateObject()
                .First()
                .Value
                .GetProperty("hash")
                .GetString();

            if (string.IsNullOrEmpty(imageHash))
                throw new Exception("Image hash not found from Meta response");

            var payloadObj = new 
            {
                name = "AI Creative",
                object_story_spec = new
                {
                    page_id = request.PageId,
                    link_data = new
                    {
                        message = request.PrimaryText ?? "Check this product!",
                        link = request.LandingPageUrl,
                        caption = request.Headline ?? "Best Product",
                        image_hash = imageHash,
                        call_to_action = new
                        {
                            type = request.CallToAction ?? "SHOP_NOW"
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(payloadObj);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestUrl = $"{url}?access_token={_settings.AccessToken}";

            var response = await _http.PostAsync(requestUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();

            Console.WriteLine("adcreatives Response: " + responseString);

            if (!response.IsSuccessStatusCode)
                throw new Exception(responseString);

            using var doc = JsonDocument.Parse(responseString);

            if (doc.RootElement.TryGetProperty("error", out var error))
                throw new Exception($"CreateCreative Error: {error}");

            return doc.RootElement.GetProperty("id").GetString();
            
        }

        // Create Ad
        private async Task<string> CreateAd(CreateAdRequest request, string adSetId, string creativeId)
        {
            var url = $"{_settings.BaseUrl}{_settings.ApiVersion}/{request.AdAccountId}/ads";

            var payload = new Dictionary<string, string>
            {
                ["name"] = request.AdName,
                ["adset_id"] = adSetId,
                ["creative"] = JsonSerializer.Serialize(new
                {
                    creative_id = creativeId
                }),
                ["status"] = "PAUSED",
                ["access_token"] = _settings.AccessToken
            };
            
            var response = await _http.PostAsync(url, new FormUrlEncodedContent(payload));
            var json = await response.Content.ReadAsStringAsync();

            Console.WriteLine("ads Response: " + json);

            if (!response.IsSuccessStatusCode)
                throw new Exception(json);

            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("error", out var error))
            {
                var message = error.GetProperty("message").GetString();
                throw new Exception($"CreateAdSet Error: {message}");
            }

            var adId = doc.RootElement.GetProperty("id").GetString();

            return adId;


        }
        public async Task<string> GetAdAccountInsights(string adAccountId)
        {
            var url =
                $"{_settings.BaseUrl}{_settings.ApiVersion}/{adAccountId}/insights" +
            $"?fields=impressions,clicks,spend,ctr&access_token={_settings.AccessToken}";

            var response = await _http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Meta API Error: {response.StatusCode}");
            }

                return await response.Content.ReadAsStringAsync();
        }
    }
}
