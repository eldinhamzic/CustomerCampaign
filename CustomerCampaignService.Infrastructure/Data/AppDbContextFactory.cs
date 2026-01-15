using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CustomerCampaignService.Infrastructure.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Ovo je folder iz kojeg pokrece� dotnet-ef
            var basePath = Directory.GetCurrentDirectory();

            // Ako appsettings nije tu, probaj u Api folderu pored (common case)
            if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
            {
                var maybeApi = Path.Combine(basePath, "CustomerCampaignService.Api");
                if (File.Exists(Path.Combine(maybeApi, "appsettings.json")))
                    basePath = maybeApi;
            }

            if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
                throw new DirectoryNotFoundException($"Ne mogu naci appsettings.json. BasePath: {basePath}");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var connStr = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connStr);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}


