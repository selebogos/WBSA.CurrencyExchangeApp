using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBSA.CurrencyExchangeApp.Services.DTOS;

namespace WBSA.CurrencyExchangeApp.Services.Abstractions
{
    public interface ICurrencyExchangeService
    {
        Task<CurrencyExchangeResponseDto> GetAsync(string lastestBaseCurrency);
        Task<CurrencyExchangeResponseDto> ConvertAndSaveAsync(CurrencyExchangeRequestDto request);
    }
}
