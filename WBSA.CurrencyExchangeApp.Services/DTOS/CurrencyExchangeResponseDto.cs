using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBSA.CurrencyExchangeApp.Services.DTOS
{
    public class CurrencyExchangeResponseDto
    {
        public bool success { get; set; }
        public string terms { get; set; }
        public string privacy { get; set; }
        public QueryDto query { get; set; }
        public InfoDto info { get; set; }
        public double result { get; set; }
    }
}
