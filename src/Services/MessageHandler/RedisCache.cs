using System.Net;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;

namespace PokeQuiz.Services.MessageHandler;

public class RedisCache(IDistributedCache distributedCache) : DelegatingHandler
{
    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return SendAsync(request, cancellationToken).Result;
    }

    public HttpResponseMessage SendInternal(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Send(request, cancellationToken);
    }

    /// <summary>
    /// Sends a request and caches the response.
    /// The response is loaded from cache if possible.
    /// </summary>
    /// <param name="request">The intercepted requests</param>
    /// <param name="cancellationToken">Cancellation token for the request</param>
    /// <returns>The response data of the request</returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var cacheKey = request.RequestUri!.PathAndQuery.TrimEnd('/');
        var cachedResponse = await distributedCache.GetStringAsync(cacheKey, cancellationToken);

        if (!string.IsNullOrEmpty(cachedResponse))
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(cachedResponse, Encoding.UTF8, "application/json")
            };
        }

        var response = await base.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return response;
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        await distributedCache.SetStringAsync(cacheKey, content, cancellationToken);

        return response;
    }
}