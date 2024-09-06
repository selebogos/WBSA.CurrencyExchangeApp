using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WBSA.CurrencyExchangeApp.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDataServices(this IServiceCollection services, IConfiguration configuration)
        {
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            var dbName = Environment.GetEnvironmentVariable("DB_NAME");
            var dbPassword= Environment.GetEnvironmentVariable("DB_ROOT_PASSWORD");

            string connnectionString = $"Server={dbHost};port=3306; Database={dbName}; Uid=root; Pwd={dbPassword};";

            services.AddDbContext<CurrencyExchangeDbContext>(opt =>
                opt.UseMySQL(connnectionString));
        }
        public static void InitializeDatabase(this IApplicationBuilder app)
        {
            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<CurrencyExchangeDbContext>();

            var pendingMigrations = context.Database.GetPendingMigrations().ToList();
            if (pendingMigrations.Any())
            {
                context.Database.Migrate();
            }
        }
    }
}
