using Google.Protobuf.WellKnownTypes;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MySqlX.XDevAPI.Common;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using WBSA.CurrencyExchangeApp.Data;
using WBSA.CurrencyExchangeApp.Data.Entities;
using WBSA.CurrencyExchangeApp.Services.Abstractions;
using WBSA.CurrencyExchangeApp.Services.Caching;
using WBSA.CurrencyExchangeApp.Services.DTOS;
using WBSA.CurrencyExchangeApp.Services.Exceptions;
using WBSA.CurrencyExchangeApp.Services.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WBSA.CurrencyExchangeApp.Services
{
    public class CurrencyExchangeService : ICurrencyExchangeService
    {
        private readonly CurrencyExchangeSettings _currencyExchangeSettings;
        private readonly CurrencyExchangeDbContext _currencyExchangeDbContext;
        private readonly IMapper _mapper;
        private readonly IRedisCacheService _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrencyExchangeService(IOptions<CurrencyExchangeSettings> options, IRedisCacheService cache,
            CurrencyExchangeDbContext currencyExchangeDbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _currencyExchangeSettings = options.Value;
            _currencyExchangeDbContext = currencyExchangeDbContext;
            _mapper = mapper;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<CurrencyExchangeResponseDto> ConvertAndSaveAsync(CurrencyExchangeRequestDto request)
        {
            
            if (!CurrencyValidator.IsValidCurrencyCode(request.BaseCurrency))
            {
                throw new CurrencyExchangeException($"{request.BaseCurrency} is not a valid currency code.");
            }
            else if (!CurrencyValidator.IsValidCurrencyCode(request.TargetCurrency))
            {
                throw new CurrencyExchangeException($"{request.TargetCurrency} is not a valid currency code.");
            }

            var cacheKey = CreateInstance(request);

           bool isFound=hasInstanceId(cacheKey);
            //check in cache first else call the API then save to database
            if(isFound)
            {
                return _cache.GetCachedData<CurrencyExchangeResponseDto>(cacheKey)?? await MakeRequestAsync(request, cacheKey);
            }

            return await MakeRequestAsync(request, cacheKey);
        }

        public async  Task<List<CurrencyExchangeResponseDto>> GetAllAsync()
        {
            return  await (from history in _currencyExchangeDbContext.CurrencyExchangeHistory
                     join q in _currencyExchangeDbContext.Query
                     on history.QueryId equals q.Id
                     join info in _currencyExchangeDbContext.Info
                     on history.InformationId equals info.Id
                     select new CurrencyExchangeResponseDto {
                         Info = _mapper.Map<InfoDto>(info),
                         Query = _mapper.Map<QueryDto>(q),
                         Result = history.Result,
                         Privacy = history.Privacy,
                         Success = history.Success,
                         Terms = history.Terms
                     }).ToListAsync() ?? new List<CurrencyExchangeResponseDto>();
        }

        private async Task SaveAsync(CurrencyExchangeHistory currencyExchangeHistory)
        {
            _currencyExchangeDbContext.Add(currencyExchangeHistory);
            await _currencyExchangeDbContext.SaveChangesAsync();
        }
        private string CreateInstance(CurrencyExchangeRequestDto request) 
        {
            return $"CurrencyExchange_{request.BaseCurrency}_{request.TargetCurrency}_{request.Amount}";
        }
        private void SetInstance(string instanceId)
        {
             byte[] buffer = System.Text.Encoding.UTF8.GetBytes(instanceId);
              _httpContextAccessor.HttpContext.Session.Set(instanceId, buffer);
        }
        private bool hasInstanceId(string instanceId)
        {
            return _httpContextAccessor.HttpContext.Session.TryGetValue(instanceId, out byte[] byteArr);
        }
        private async Task<CurrencyExchangeResponseDto> MakeRequestAsync(CurrencyExchangeRequestDto request,string cacheKey)
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

                if (data is null)
                    throw new CurrencyExchangeException($"Cannot convert currency");
                else if (data.Info is null)
                    throw new CurrencyExchangeException($"Failed convert currency");

                var currencyExchangeHistory = _mapper.Map<CurrencyExchangeHistory>(data);

                await SaveAsync(currencyExchangeHistory);
                SetInstance(cacheKey);
                _cache.SetCachedData(cacheKey, data, TimeSpan.FromSeconds(20));

                return data?? new();
            }
        }
    }
}
