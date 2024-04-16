using System.Net;
using System.Text;

/// <summary>
/// Handles caching <see cref="HttpResponseMessage"/> as (JSON) files
/// </summary>
internal class FileCache : DelegatingHandler
{
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return SendAsync(request, cancellationToken).Result;
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
        var cachePath = Path.Join(Directory.GetCurrentDirectory(), "Cache", request.RequestUri!.PathAndQuery)
            .TrimEnd('/') + ".json";
        SemaphoreSlim fileLock = _semaphoreSlim;

        if (File.Exists(cachePath))
        {
            string content;

            // used to prevent reading a file that's not (completely) written.
            // can happen during parallel requests to the same resource, e.g. requesting the same (uncached) move twice.
            await fileLock.WaitAsync(cancellationToken);
            try
            {
                content = await File.ReadAllTextAsync(cachePath, cancellationToken);
            }
            finally
            {
                fileLock.Release();
            }

            if (!string.IsNullOrWhiteSpace(content))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
            }
        }

        var response = await base.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return response;
        }

        await fileLock.WaitAsync(cancellationToken);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(cachePath)!);
            await File.WriteAllTextAsync(cachePath, await response.Content.ReadAsStringAsync(cancellationToken),
                cancellationToken);
        }
        finally
        {
            fileLock.Release();
        }

        return response;
    }
};