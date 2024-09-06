using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBSA.CurrencyExchangeApp.Services.Exceptions
{
    public class CurrencyExchangeException : Exception
    {
        public CurrencyExchangeException() : base() { }

        public CurrencyExchangeException(string message) : base(message) { }

        public CurrencyExchangeException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
