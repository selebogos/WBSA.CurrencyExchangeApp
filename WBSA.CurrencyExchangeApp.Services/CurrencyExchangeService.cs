using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Collections;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using WBSA.CurrencyExchangeApp.Data;
using WBSA.CurrencyExchangeApp.Data.Entities;
using WBSA.CurrencyExchangeApp.Services.Abstractions;
using WBSA.CurrencyExchangeApp.Services.Caching;
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
        private readonly IRedisCacheService _cache;
        public CurrencyExchangeService(IOptions<CurrencyExchangeSettings> options, IRedisCacheService cache,
            CurrencyExchangeDbContext currencyExchangeDbContext, IMapper mapper)
        {
            _currencyExchangeSettings = options.Value;
            _currencyExchangeDbContext = currencyExchangeDbContext;
            _mapper = mapper;
            _cache = cache;
        }
        public async Task<CurrencyExchangeResponseDto> ConvertAndSaveAsync(CurrencyExchangeRequestDto request)
        {
            var cacheKey = _currencyExchangeSettings.CacheKey;

            //check in cache first else call the API then save to database
            CurrencyExchangeResponseDto currencyExchangeResponse = _cache.GetCachedData<CurrencyExchangeResponseDto>(cacheKey);
            if (currencyExchangeResponse is null)
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(_currencyExchangeSettings.HostUrl);
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await httpClient.GetAsync($"{_currencyExchangeSettings.ConvertEndPoint}?from={request.BaseCurrency}&to={request.TargetCurrency}&amount={request.Amount}&access_key={_currencyExchangeSettings.CurrrencyAPIKey}");
                    if (!response.IsSuccessStatusCode)
                        throw new CurrencyExchangeException($"Cannot convert currency, status code: {response.StatusCode}");

                    var data = await response.Content.ReadFromJsonAsync<CurrencyExchangeResponseDto>() ?? null;

                    if (data == null)
                        throw new CurrencyExchangeException($"Cannot convert currency");

                    var currencyExchangeHistory = _mapper.Map<CurrencyExchangeHistory>(request);
                    currencyExchangeHistory.ExchangeRate = data.result;

                    await SaveAsync(currencyExchangeHistory);
                    _cache.SetCachedData(_currencyExchangeSettings.CacheKey, data, TimeSpan.FromSeconds(360));

                    return data ?? new();
                }

            }
            return currencyExchangeResponse;
           
        }

        public Task<IEnumerable<CurrencyExchangeResponseDto>> GetAsync()
        {
            return _currencyExchangeDbContext.CurrencyExchangeHistory.Select(x => new CurrencyExchangeResponseDto { 
                
            });
        }

        private async Task SaveAsync(CurrencyExchangeHistory currencyExchangeHistory)
        {
            _currencyExchangeDbContext.Add(currencyExchangeHistory);
            await _currencyExchangeDbContext.SaveChangesAsync();
        }

   
    }
}
