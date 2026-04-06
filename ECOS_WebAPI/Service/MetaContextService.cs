using ECOS_WebAPI.Data;
using ECOS_WebAPI.Models;
using ECOS_WebAPI.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ECOS_WebAPI.Service
{
    public class MetaContextService : IMetaContextService
    {
        private readonly AppDbContext _db;
        private readonly MetaSettings _settings;

        public MetaContextService(AppDbContext db, IOptions<MetaSettings> settings)
        {
            _db = db;
            _settings = settings.Value;
        }

        public async Task<MetaSettings> GetMetaContextAsync(string userId)
        {
            var config = await _db.MetaConfigs
            .FirstOrDefaultAsync(x => x.UserId == userId);

            //if (config == null)
            //    throw new Exception("Meta not connected");

            //return new MetaSettings
            //{
            //    AdAccountId = _settings.AdAccountId,
            //    PageId = _settings.PageId
            //};

            var metaSetting = new MetaSettings
            {
                PixelId = _settings.PixelId,

                AdAccountId = config?.AdAccountId ?? _settings.AdAccountId,
                PageId = config?.PageId ?? _settings.PageId,

                AccessToken = config?.AccessToken ?? _settings.AccessToken,

                ApiVersion = _settings.ApiVersion,
                BaseUrl = _settings.BaseUrl,
                TestCode = _settings.TestCode
            };
            return metaSetting;
        }



        
    }
}
