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
         IMapper _mapper)
        : ControllerBase
    {

        [HttpGet("history")]
        public async Task<IActionResult> GetByDate(string date)
        {
            var item = await _currencyExchangeService.GetAsync(date);
            return Ok(item);
        }

        [HttpGet("convert")]
        public async Task<IActionResult> Convert(CurrencyExchangeRequest request)
        {
            var currencyRequest=_mapper.Map<CurrencyExchangeRequestDto>(request);
            var item = await _currencyExchangeService.ConvertAsync(currencyRequest);
            return Ok(item);
        }
    }
}
