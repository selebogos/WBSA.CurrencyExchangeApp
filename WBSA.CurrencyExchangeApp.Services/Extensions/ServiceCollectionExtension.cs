
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WBSA.CurrencyExchangeApp.Services.Abstractions;
using WBSA.CurrencyExchangeApp.Services.Caching;
using WBSA.CurrencyExchangeApp.Services.DTOS;

namespace WBSA.CurrencyExchangeApp.Services.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddCurrencyExchangeSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var currencyExchangeSettingSection = configuration.GetSection("CurrencyExchangeSettings");
            services.Configure<CurrencyExchangeSettings>(currencyExchangeSettingSection);
        }
        public static void AddCurrencyExchangeServices(this IServiceCollection services)
        {
            services.AddScoped<ICurrencyExchangeService, CurrencyExchangeService>();
            services.AddScoped<IRedisCacheService, RedisCacheService>();



        }
    }
}
