using System.Net;
using System.Text;
using System.Text.Json;

class FileCache: DelegatingHandler
{
    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return SendAsync(request, cancellationToken).Result;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var cachePath = Path.Join(Directory.GetCurrentDirectory(), "Cache", request.RequestUri!.PathAndQuery).TrimEnd('/') + ".json";
        if (File.Exists(cachePath))
        {
            // TODO: try with stream content
            var content = File.ReadAllText(cachePath);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };
        }
        var response = await base.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return response;
        }
    
        Directory.CreateDirectory(Path.GetDirectoryName(cachePath)!);
        await File.WriteAllTextAsync(cachePath, await response.Content.ReadAsStringAsync(cancellationToken), cancellationToken);
        
        return response;
    }
};