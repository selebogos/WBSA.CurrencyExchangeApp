using System.ComponentModel.DataAnnotations;

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
        [Required]
        [Range(0.01, int.MaxValue, ErrorMessage = "Value Must Bigger Than {1}")]
        public decimal Amount { get; set; } = 0.00m;
    }
}
