using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WBSA.CurrencyExchangeApp.API.Models;
using WBSA.CurrencyExchangeApp.Services.Abstractions;
using WBSA.CurrencyExchangeApp.Services.DTOS;

namespace WBSA.CurrencyExchangeApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyExchangersController(ICurrencyExchangeService _currencyExchangeService,
         IMapper _mapper): ControllerBase
    {
        /// <summary>
        /// Gets All History data 
        /// </summary>
        /// <returns>The list of Currency Exchange History.</returns>
        // GET: api/history
        [HttpGet("history")]
        public async Task<IActionResult> GetAll()
        {
            var item = await _currencyExchangeService.GetAllAsync();
            return Ok(item);
        }
        /// <summary>
        /// Converts from oe currency to the other
        /// </summary>
        /// <returns>conversion results</returns>
        // POST: api/convert
        [HttpPost("convert")]
        public async Task<IActionResult> Convert(string baseCurrency,string targetCurrency,string amount)
        {
            var currencyRequest=new CurrencyExchangeRequestDto { BaseCurrency=baseCurrency,TargetCurrency=targetCurrency,Amount=amount};
            var item = await _currencyExchangeService.ConvertAndSaveAsync(currencyRequest);
            return Ok(item);
        }
    }
}
