using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using WBSA.CurrencyExchangeApp.API.Helper;

namespace WBSA.CurrencyExchangeApp.API.Models
{
    public class CurrencyExchangeRequest
    {
 
        public string? BaseCurrency { get; set; }
        public string? TargetCurrency { get; set; }
        public decimal? Amount { get; set; } = 0;

    }
}
