using ECOS_WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECOS_WebAPI.Data
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<MetaConfig> MetaConfigs { get; set; }
    }
}
