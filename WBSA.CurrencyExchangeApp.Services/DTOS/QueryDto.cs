using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBSA.CurrencyExchangeApp.Services.DTOS
{
    public class QueryDto
    {
        public string from { get; set; }
        public string to { get; set; }
        public int amount { get; set; }
    }
}
