using System.Collections.Concurrent;
using System.Net;
using System.Text;

class FileCache : DelegatingHandler
{
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
    private ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return SendAsync(request, cancellationToken).Result;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var cachePath = Path.Join(Directory.GetCurrentDirectory(), "Cache", request.RequestUri!.PathAndQuery)
            .TrimEnd('/') + ".json";
        SemaphoreSlim fileLock = _semaphoreSlim;

        if (File.Exists(cachePath))
        {
            string content;
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