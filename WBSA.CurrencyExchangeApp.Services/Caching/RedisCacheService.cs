using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WBSA.CurrencyExchangeApp.Services.Abstractions;

namespace WBSA.CurrencyExchangeApp.Services.Caching
{
    public class RedisCacheService(IDistributedCache? _cache) : IRedisCacheService
    {

        public T GetCachedData<T>(string key)
        {
            var jsonData = _cache.GetString(key);

            if (jsonData is null)
                return default(T);

            return JsonSerializer.Deserialize<T>(jsonData);
        }
        public void SetCachedData<T>(string key, T data, TimeSpan cacheDuration)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.Add(cacheDuration)
            };

            var jsonData = JsonSerializer.Serialize(data);
            _cache.SetString(key, jsonData, options);
        }
    }
}
