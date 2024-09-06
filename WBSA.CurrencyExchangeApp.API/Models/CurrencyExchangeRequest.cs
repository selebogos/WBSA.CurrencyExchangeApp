using System.ComponentModel.DataAnnotations;

namespace WBSA.CurrencyExchangeApp.API.Models
{
    public class CurrencyExchangeRequest
    {
        public string BaseCurrency { get; set; }
        public string TargetCurrency { get; set; }
        [Range(0.01, int.MaxValue, ErrorMessage = "Value Must Bigger Than {1}")]
        public decimal Amount { get; set; }
    }
}
