using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBSA.CurrencyExchangeApp.Data.Entities
{
    [Table("Information")]
    public class Information
    {
        public int timestamp { get; set; }
        public double quote { get; set; }
    }
}
