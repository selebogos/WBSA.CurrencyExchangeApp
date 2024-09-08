using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WBSA.CurrencyExchangeApp.Data.Entities
{
    [Table("Information")]
    public class Information
    {
        [Key]
        public int Id { get; set; }
        public int Timestamp { get; set; }
        public string Quote { get; set; }
    }
}
