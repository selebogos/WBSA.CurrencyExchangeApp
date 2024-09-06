using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBSA.CurrencyExchangeApp.Data.Entities
{
    [Table("CurrencyExchangeHistory")]
    public class CurrencyExchangeHistory
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string BaseCurrency { get; set; }
        [Required]
        public string TargetCurrency { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public double ExchangeRate { get; set; }
        [Required]
        public DateTime Date { get; set; }
    }
}
