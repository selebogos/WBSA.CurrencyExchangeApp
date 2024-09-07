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
        Task<List<CurrencyExchangeResponseDto>> GetAllAsync();
        Task<CurrencyExchangeResponseDto> ConvertAndSaveAsync(CurrencyExchangeRequestDto request);
    }
}
