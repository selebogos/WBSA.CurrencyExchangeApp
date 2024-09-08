using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using WBSA.CurrencyExchangeApp.API.Helper;

namespace WBSA.CurrencyExchangeApp.API.Models
{
    public class CurrencyExchangeRequest
    {
        [Required]
        [StringLength(3, ErrorMessage = "{0} length must be  {1}.", MinimumLength = 3)]
        public string BaseCurrency { get; set; }
        [Required]
        [StringLength(3, ErrorMessage = "{0} length must be  {1}.", MinimumLength = 3)]
        public string TargetCurrency { get; set; }
        [Required,Description("Amount must be greater than zero")]
        [MinValue(0.00000000001)]
        public decimal Amount { get; set; } = 0;
    }
}
