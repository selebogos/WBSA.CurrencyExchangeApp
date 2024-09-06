using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBSA.CurrencyExchangeApp.Services.DTOS
{
    public class CurrencyExchangeSettings
    {
        public string CurrrencyAPIKey { get; set; }
        public string HostUrl { get; set; }
        public string ConvertEndPoint { get; set; }
        public string CacheKey { get; set; }
    }
}
