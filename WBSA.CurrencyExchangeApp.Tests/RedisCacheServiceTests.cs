using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text.Json;
using WBSA.CurrencyExchangeApp.Services.Caching;
using System.Text;

namespace WBSA.CurrencyExchangeApp.Tests
{
    [TestFixture]
    public class RedisCacheServiceTests
    {
        private Mock<IDistributedCache> _mockDistributedCache;
        private RedisCacheService _redisCacheService;

        [SetUp]
        public void SetUp()
        {
            _mockDistributedCache = new Mock<IDistributedCache>();
            _redisCacheService = new RedisCacheService(_mockDistributedCache.Object);
        }

        #region GetCachedData Tests

        [Test]
        public void GetCachedData_WhenDataExistsInCache_ReturnsDeserializedData()
        {
            var key = "myKey";
            var cachedData = new TestData { Id = 1, Name = "Hello Selebogo" };
            var serializedData = JsonSerializer.Serialize(cachedData);
            byte[] byteArray = Encoding.UTF8.GetBytes(serializedData);
            _mockDistributedCache.Setup(c => c.Get(key)).Returns(byteArray);

            var result = _redisCacheService.GetCachedData<TestData>(key);
            Assert.IsNotNull(result);
            Assert.AreEqual(cachedData.Id, result.Id);
            Assert.AreEqual(cachedData.Name, result.Name);
        }

        [Test]
        public void GetCachedData_WhenDataDoesNotExistInCache_ReturnsDefault()
        {
            var key = "myKey";
            string data = null;
            byte[] byteArray = Encoding.UTF8.GetBytes("");
            _mockDistributedCache.Setup(c => c.Get(key)).Returns(byteArray);
            var result = _redisCacheService.GetCachedData<TestData>(key);
            Assert.IsNull(result); // Since TestData is a reference type, default(T) should return null
        }

        #endregion

        #region SetCachedData Tests

        [Test]
        public void SetCachedData_CallsSetStringWithSerializedData()
        {
            var key = "myKey";
            var cacheDuration = TimeSpan.FromMinutes(10);
            var testData = new TestData { Id = 1, Name = "Hello Selebogo" };
            var serializedData = JsonSerializer.Serialize(testData);
            _redisCacheService.SetCachedData(key, testData, cacheDuration);

            byte[] byteArray = Encoding.UTF8.GetBytes(serializedData);
            _mockDistributedCache.Verify(c => c.Set(
                key,
               byteArray,
                It.Is<DistributedCacheEntryOptions>(opts => opts.AbsoluteExpiration.HasValue
                    && opts.AbsoluteExpiration.Value <= DateTime.Now.Add(cacheDuration))
            ), Times.Once);
        }

        #endregion
    }
    public class TestData
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}