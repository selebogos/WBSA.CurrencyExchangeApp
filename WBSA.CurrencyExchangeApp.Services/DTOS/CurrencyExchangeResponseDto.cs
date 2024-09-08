using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WBSA.CurrencyExchangeApp.Services.DTOS
{
    public class CurrencyExchangeResponseDto
    {
        public bool Success { get; set; }
        public string Terms { get; set; }
        public string Privacy { get; set; }
        public QueryDto Query { get; set; }
        public InfoDto Info { get; set; }
        public string Result { get; set; }
        public ErrorDto Error { get; set; }
    }
}
