﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WBSA.CurrencyExchangeApp.Services.DTOS
{
    public class QueryDto
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Amount { get; set; }
    }
}
