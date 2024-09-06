using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WBSA.CurrencyExchangeApp.Services.Abstractions;
using WBSA.CurrencyExchangeApp.Services.DTOS;
using WBSA.CurrencyExchangeApp.Services.Exceptions;

namespace WBSA.CurrencyExchangeApp.Services
{
    public class CurrencyExchangeService : ICurrencyExchangeService
    {
        private readonly CurrencyExchangeSettings _currencyExchangeSettings;
        public CurrencyExchangeService(IOptions<CurrencyExchangeSettings> options)
        {
            _currencyExchangeSettings = options.Value;
        }
        public async Task<CurrencyExchangeResponseDto> ConvertAsync(CurrencyExchangeRequestDto request)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(_currencyExchangeSettings.CurrrencyAPIKey);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await httpClient.GetAsync($"{_currencyExchangeSettings.HostUrl}/api/convert?from={request.BaseCurrency}&to={request.TargetCurrency}&amaount={request.Amount}&access_key={_currencyExchangeSettings.CurrrencyAPIKey}");
                if (!response.IsSuccessStatusCode)
                    throw new CurrencyExchangeException($"Cannot convert currency, status code: {response.StatusCode}");
                
                return await response.Content.ReadFromJsonAsync<CurrencyExchangeResponseDto>() ?? new();
            }
        }

        public Task<CurrencyExchangeResponseDto> GetAsync(string lastestBaseCurrency)
        {
            throw new NotImplementedException();
        }

        public Task<CurrencyExchangeResponseDto> SaveAsync()
        {
            throw new NotImplementedException();
        }
    }
}
