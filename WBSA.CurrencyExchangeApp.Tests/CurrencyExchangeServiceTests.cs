using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using WBSA.CurrencyExchangeApp.Data;
using WBSA.CurrencyExchangeApp.Data.Entities;
using WBSA.CurrencyExchangeApp.Services;
using WBSA.CurrencyExchangeApp.Services.Abstractions;
using WBSA.CurrencyExchangeApp.Services.DTOS;
using WBSA.CurrencyExchangeApp.Services.Exceptions;

namespace WBSA.CurrencyExchangeApp.Tests
{
    public class CurrencyExchangeServiceTests
    {
        private Mock<IOptions<CurrencyExchangeSettings>> _mockOptions;
        private Mock<IRedisCacheService> _mockCache;
        private Mock<CurrencyExchangeDbContext> _mockDbContext;
        private Mock<IMapper> _mockMapper;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private CurrencyExchangeService _service;
        private Mock<HttpContext> _mockHttpContext;
        private Mock<ISession> _mockSession;

        private Mock<DbSet<CurrencyExchangeHistory>> _mockCurrencyExchangeHistoryDbSet;
        private Mock<DbSet<WBSA.CurrencyExchangeApp.Data.Entities.Query>> _mockQueryDbSet;
        private Mock<DbSet<Information>> _mockInfoDbSet;


        [SetUp]
        public void SetUp()
        {
            _mockCurrencyExchangeHistoryDbSet = new Mock<DbSet<CurrencyExchangeHistory>>();
            _mockQueryDbSet = new Mock<DbSet<WBSA.CurrencyExchangeApp.Data.Entities.Query>>();
            _mockInfoDbSet = new Mock<DbSet<Information>>();

            _mockHttpContext = new Mock<HttpContext>();
            _mockSession = new Mock<ISession>();
            _mockOptions = new Mock<IOptions<CurrencyExchangeSettings>>();
            _mockCache = new Mock<IRedisCacheService>();
            _mockDbContext = new Mock<CurrencyExchangeDbContext>();
            _mockMapper = new Mock<IMapper>();
       
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            _mockHttpContext.Setup(ctx => ctx.Session).Returns(_mockSession.Object);
            _mockHttpContextAccessor.Setup(h => h.HttpContext).Returns(_mockHttpContext.Object);

            //_mockDbContext.Setup(db => db.CurrencyExchangeHistory).Returns(_mockCurrencyExchangeHistoryDbSet.Object);
           //_mockDbContext.Setup(db => db.Query).Returns(_mockQueryDbSet.Object);
            //_mockDbContext.Setup(db => db.Info).Returns(_mockInfoDbSet.Object);

            _mockOptions.Setup(opt => opt.Value).Returns(new CurrencyExchangeSettings
            {
                HostUrl = "https://api.currencylayer.com",
                ConvertEndPoint = "/api/convert",
                CurrrencyAPIKey = "18d48e3453acbb46a5da679bef299ade",
                CacheKey= "CurrencyExchange_"
            });

            _service = new CurrencyExchangeService(
                _mockOptions.Object,
                _mockCache.Object,
                _mockDbContext.Object,
                _mockMapper.Object,
                _mockHttpContextAccessor.Object
            );
        }
        [Test]
        public void ConvertAndSaveAsync_InvalidBaseCurrency_ThrowsCurrencyExchangeException()
        {
            var request = new CurrencyExchangeRequestDto
            {
                BaseCurrency = "INVALID",
                TargetCurrency = "ZAR",
                Amount = "100"
            };

            Assert.ThrowsAsync<CurrencyExchangeException>(async () => await _service.ConvertAndSaveAsync(request));
        }

        [Test]
        public void ConvertAndSaveAsync_AmountLessThanOrEqualToZero_ThrowsCurrencyExchangeException()
        {
            var request = new CurrencyExchangeRequestDto
            {
                BaseCurrency = "ZAR",
                TargetCurrency = "EUR",
                Amount = "-1"
            };

            Assert.ThrowsAsync<CurrencyExchangeException>(async () => await _service.ConvertAndSaveAsync(request));
        }

        [Test]
        public void ConvertAndSaveAsync_SameBaseAndTargetCurrency_ThrowsCurrencyExchangeException()
        {
            var request = new CurrencyExchangeRequestDto
            {
                BaseCurrency = "ZAR",
                TargetCurrency = "ZAR", // Same as base currency
                Amount = "100"
            };

            Assert.ThrowsAsync<CurrencyExchangeException>(async () => await _service.ConvertAndSaveAsync(request));
        }
        [Test]
        public void ConvertAndSaveAsync_AmountNotANumber_ThrowsCurrencyExchangeException()
        {
            var request = new CurrencyExchangeRequestDto
            {
                BaseCurrency = "ZAR",
                TargetCurrency = "EUR",
                Amount = "I am not a number"
            };

            Assert.ThrowsAsync<CurrencyExchangeException>(async () => await _service.ConvertAndSaveAsync(request));
        }
        [Test]
        public async Task ConvertAndSaveAsync_ApiCall_SuccessfulResponse()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("https://api.currencylayer.com/api/convert")
                    .Respond("application/json", "{ 'success': true, 'result': 0.85 }"); 

            var client = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("https://api.currencylayer.com")
            };

            var result = await _service.ConvertAndSaveAsync(new CurrencyExchangeRequestDto
            {
                BaseCurrency = "ZAR",
                TargetCurrency = "EUR",
                Amount = "50"
            });

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
        }

        [Test]
        public async Task ConvertAndSaveAsync_CacheHit_ReturnsCachedData()
        {
            var request = new CurrencyExchangeRequestDto
            {
                BaseCurrency = "USD",
                TargetCurrency = "EUR",
                Amount = "100"
            };

            var cachedResponse = new CurrencyExchangeResponseDto
            {
                Success = true,
                Result = "0.85"
            };

            _mockCache.Setup(c => c.GetCachedData<CurrencyExchangeResponseDto>(It.IsAny<string>()))
                      .Returns(cachedResponse);

            _mockHttpContextAccessor.Setup(h => h.HttpContext.Session.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]>.IsAny))
                                    .Returns(true);

            var result = await _service.ConvertAndSaveAsync(request);

            Assert.AreEqual(cachedResponse, result);
        }
        [Test]
        public async Task ConvertAndSaveAsync_CacheDataNotFound_CallsApiAndSavesData()
        {
            var request = new CurrencyExchangeRequestDto
            {
                BaseCurrency = "USD",
                TargetCurrency = "EUR",
                Amount = "100"
            };
            CurrencyExchangeResponseDto noData =null;
            _mockCache.Setup(c => c.GetCachedData<CurrencyExchangeResponseDto>(It.IsAny<string>()))
                      .Returns(noData); // Cache miss

            _mockHttpContextAccessor.Setup(h => h.HttpContext.Session.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]>.IsAny))
                                    .Returns(false);

            var result = await _service.ConvertAndSaveAsync(request);

            Assert.IsNotNull(result); 
        }

        [Test]
        public async Task GetAllAsync_ReturnsListOfCurrencyExchangeResponseDto()
        {

            var options = new DbContextOptionsBuilder<CurrencyExchangeDbContext>()
           .UseInMemoryDatabase(databaseName: "currency_exchange")
           .Options;

            using (var context = new CurrencyExchangeDbContext(options))
            {
                context.CurrencyExchangeHistory.Add(new CurrencyExchangeHistory {
                    Id=1,
                    QueryId = 1,
                    InformationId = 1,
                    Result = "1428.308", 
                    Privacy = "https://currencylayer.com/privacy",
                    Success = true, Terms = "https://currencylayer.com/terms",
                  
                    Query = new WBSA.CurrencyExchangeApp.Data.Entities.Query { Id = 1, From = "USD", To = "EUR", Amount = "100" },
                    Info=new Information { Id = 1, Timestamp = 1725745816, Quote = "17.85385" }
                });

                context.CurrencyExchangeHistory.Add(new CurrencyExchangeHistory
                {
                    Id=2,
                    QueryId = 2,
                    InformationId = 2,
                    Result = "1428.308",
                    Privacy = "https://currencylayer.com/privacy",
                    Success = true,
                    Terms = "https://currencylayer.com/terms",
                    Query = new WBSA.CurrencyExchangeApp.Data.Entities.Query { Id = 2, From = "USD", To = "EUR", Amount = "100" },
                    Info = new Information { Id = 2, Timestamp = 1725745816, Quote = "17.85385" }
                });

                context.CurrencyExchangeHistory.Add(new CurrencyExchangeHistory
                {
                    Id=3,
                    QueryId = 3,
                    InformationId = 3,
                    Result = "1428.308",
                    Privacy = "https://currencylayer.com/privacy",
                    Success = true,
                    Terms = "https://currencylayer.com/terms",
                    Query = new WBSA.CurrencyExchangeApp.Data.Entities.Query { Id = 3, From = "USD", To = "EUR", Amount = "100" },
                    Info = new Information { Id = 3, Timestamp = 1725745816, Quote = "17.85385" }
                });
                context.SaveChanges();
            }
            using (var context = new CurrencyExchangeDbContext(options))
            {
                CurrencyExchangeService currencyExchangeService = new CurrencyExchangeService(
                   _mockOptions.Object,
                   _mockCache.Object,
                   context,
                   _mockMapper.Object,
                   _mockHttpContextAccessor.Object
                );
               
                var data = await currencyExchangeService.GetAllAsync();
                Assert.That(data.Count, Is.EqualTo(3));
            }

        }

    }

}
