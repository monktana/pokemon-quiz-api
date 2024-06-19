using System.Net;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using RedisCache = PokeQuiz.Services.MessageHandler.RedisCache;

namespace PokeQuiz.UnitTests.Services;

public class RedisCacheTests
{
    private readonly Mock<IDistributedCache> _mockDistributedCache = new();

    [Fact]
    public void SendAsync_CacheMiss_CallsBaseSendAsync()
    {
        _mockDistributedCache.Setup(cache => cache.GetAsync("/api/v2/pokemon/bulbasaur", It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        _mockDistributedCache.Setup(cache => cache.SetAsync("/api/v2/pokemon/bulbasaur", Encoding.UTF8.GetBytes("{}"), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()));

        var redisCache = new RedisCache(_mockDistributedCache.Object);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://pokeapi.co/api/v2/pokemon/bulbasaur");
        redisCache.InnerHandler = new FakeHttpClientHandler();

        redisCache.SendInternal(httpRequestMessage, CancellationToken.None);
        _mockDistributedCache.Verify(x => x.GetAsync("/api/v2/pokemon/bulbasaur", It.IsAny<CancellationToken>()), Times.Once);
        _mockDistributedCache.Verify(x => x.SetAsync("/api/v2/pokemon/bulbasaur", Encoding.UTF8.GetBytes("{}"), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void SendAsync_CacheHit_ReturnsCachedResponse()
    {
        _mockDistributedCache.Setup(cache => cache.GetAsync("/api/v2/pokemon/bulbasaur", CancellationToken.None))
            .ReturnsAsync(Encoding.UTF8.GetBytes("{}"));

        var fakeHttpClientHandler = new FakeHttpClientHandler();
        var redisCache = new RedisCache(_mockDistributedCache.Object);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://pokeapi.co/api/v2/pokemon/bulbasaur");
        redisCache.InnerHandler = fakeHttpClientHandler;

        var response = redisCache.SendInternal(httpRequestMessage, CancellationToken.None);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(0, fakeHttpClientHandler.CallCount);
    }
}

public class FakeHttpClientHandler : HttpClientHandler
{
    public int CallCount { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        CallCount += 1;
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json")
        });
    }
}