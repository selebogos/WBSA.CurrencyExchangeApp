using MapsterMapper;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WBSA.CurrencyExchangeApp.Data;
using WBSA.CurrencyExchangeApp.Data.Entities;
using WBSA.CurrencyExchangeApp.Services.Abstractions;
using WBSA.CurrencyExchangeApp.Services.DTOS;
using WBSA.CurrencyExchangeApp.Services.Exceptions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WBSA.CurrencyExchangeApp.Services
{
    public class CurrencyExchangeService : ICurrencyExchangeService
    {
        private readonly CurrencyExchangeSettings _currencyExchangeSettings;
        private readonly CurrencyExchangeDbContext _currencyExchangeDbContext;
        private readonly IMapper _mapper;
        public CurrencyExchangeService(IOptions<CurrencyExchangeSettings> options, CurrencyExchangeDbContext currencyExchangeDbContext, IMapper mapper)
        {
            _currencyExchangeSettings = options.Value;
            _currencyExchangeDbContext = currencyExchangeDbContext;
            _mapper = mapper;
        }
        public async Task<CurrencyExchangeResponseDto> ConvertAndSaveAsync(CurrencyExchangeRequestDto request)
        {
            //check in cache first else call the API then save to database
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(_currencyExchangeSettings.CurrrencyAPIKey);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await httpClient.GetAsync($"{_currencyExchangeSettings.HostUrl}/api/convert?from={request.BaseCurrency}&to={request.TargetCurrency}&amaount={request.Amount}&access_key={_currencyExchangeSettings.CurrrencyAPIKey}");
                if (!response.IsSuccessStatusCode)
                    throw new CurrencyExchangeException($"Cannot convert currency, status code: {response.StatusCode}");

                var data = await response.Content.ReadFromJsonAsync<CurrencyExchangeResponseDto>() ?? null;

                if(data==null)
                    throw new CurrencyExchangeException($"Cannot convert currency");

                var currencyExchangeHistory = _mapper.Map<CurrencyExchangeHistory>(request);
                currencyExchangeHistory.ExchangeRate = data.result;

                await SaveAsync(currencyExchangeHistory);

                return data ?? new();
            }
        }

        public Task<CurrencyExchangeResponseDto> GetAsync(string lastestBaseCurrency)
        {
            throw new NotImplementedException();
        }

        private async Task SaveAsync(CurrencyExchangeHistory currencyExchangeHistory)
        {
            _currencyExchangeDbContext.Add(currencyExchangeHistory);
            await _currencyExchangeDbContext.SaveChangesAsync();
        }
    }
}
