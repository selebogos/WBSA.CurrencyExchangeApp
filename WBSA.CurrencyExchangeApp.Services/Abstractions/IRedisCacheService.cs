using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBSA.CurrencyExchangeApp.Services.Abstractions
{
    public interface IRedisCacheService
    {

        public T GetCachedData<T>(string key);
        public void SetCachedData<T>(string key, T data, TimeSpan cacheDuration);
    }
}
