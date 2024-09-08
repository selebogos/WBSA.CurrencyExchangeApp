using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace WBSA.CurrencyExchangeApp.Data.Entities
{
    [Table("CurrencyExchangeHistory")]
    public class CurrencyExchangeHistory
    {
        [Key]
        public int Id { get; set; }
        public int InformationId { get; set; }
        public int QueryId { get; set; }
        public bool Success { get; set; }
        public string Terms { get; set; }
        public string Privacy { get; set; }
        public string Result { get; set; }

        [ForeignKey("InformationId")]
        public virtual Information Info { get; set; }

        [ForeignKey("QueryId")]
        public virtual Query Query { get; set; }
    }
}
