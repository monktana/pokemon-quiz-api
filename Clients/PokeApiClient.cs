using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using PokeQuiz.Models.PokeApi;

namespace PokeQuiz.Clients;

/// <summary>
/// Gets data from the PokeAPI.
/// </summary>
public class PokeApiClient : IDisposable
{
    /// <summary>
    /// The default `User-Agent` header value used by instances of <see cref="PokeApiClient"/>.
    /// </summary>
    public static readonly ProductHeaderValue DefaultUserAgent = GetDefaultUserAgent();

    private readonly Uri _baseUri = new Uri("https://pokeapi.co/api/v2/");

    private readonly HttpClient _client;

    /// <summary>
    /// Construct accepting a HttpClient.
    /// See https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
    /// </summary>
    /// <param name="httpClient">HttpClient implementation</param>
    public PokeApiClient(HttpClient httpClient)
    {
        _client = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _client.BaseAddress = _baseUri;
    }

    /// <summary>
    /// Close resources
    /// </summary>
    public void Dispose()
    {
        _client.Dispose();
    }

    private static ProductHeaderValue GetDefaultUserAgent()
    {
        var version = typeof(PokeApiClient).Assembly.GetName().Version;
        return new ProductHeaderValue("PokeApi", $"{version.Major}.{version.Minor}");
    }

    /// <summary>
    /// Send a request to the api and serialize the response into the specified type
    /// </summary>
    /// <typeparam name="T">The type of resource</typeparam>
    /// <param name="apiParam">The name or id of the resource</param>
    /// <param name="cancellationToken">Cancellation token for the request; not utilized if data has been cached</param>
    /// <exception cref="HttpRequestException">Something went wrong with your request</exception>
    /// <returns>An instance of the specified type with data from the request</returns>
    private async Task<T> GetResourcesWithParamsAsync<T>(string apiParam, CancellationToken cancellationToken)
        where T : ResourceBase
    {
        // check for case sensitive API endpoint
        bool isApiEndpointCaseSensitive = IsApiEndpointCaseSensitive<T>();
        string sanitizedApiParam = isApiEndpointCaseSensitive ? apiParam : apiParam.ToLowerInvariant();
        string apiEndpoint = GetApiEndpointString<T>();

        return await GetAsync<T>($"{apiEndpoint}/{sanitizedApiParam}/", cancellationToken);
    }

    /// <summary>
    /// Gets a resource from a navigation url
    /// </summary>
    /// <typeparam name="T">The type of resource</typeparam>
    /// <param name="url">Navigation url</param>
    /// <param name="cancellationToken">Cancellation token for the request; not utilized if data has been cached</param>
    /// <exception cref="NotSupportedException">Navigation url doesn't contain the resource id</exception>
    /// <returns>The object of the resource</returns>
    private async Task<T> GetResourceByUrlAsync<T>(string url, CancellationToken cancellationToken)
        where T : ResourceBase
    {
        // need to parse out the id in order to check if it's cached.
        // navigation urls always use the id of the resource
        string trimmedUrl = url.TrimEnd('/');
        string resourceId = trimmedUrl.Substring(trimmedUrl.LastIndexOf('/') + 1);

        if (!int.TryParse(resourceId, out _))
        {
            // not sure what to do here...
            throw new NotSupportedException($"Navigation url '{url}' is in an unexpected format");
        }

        return await GetResourcesWithParamsAsync<T>(resourceId, cancellationToken);
    }

    /// <summary>
    /// Gets a resource by id.
    /// </summary>
    /// <typeparam name="T">The type of resource</typeparam>
    /// <param name="id">Id of resource</param>
    /// <returns>The object of the resource</returns>
    public async Task<T> GetResourceAsync<T>(int id) where T : ResourceBase
    {
        return await GetResourceAsync<T>(id, CancellationToken.None);
    }

    /// <summary>
    /// Gets a resource by id
    /// </summary>
    /// <typeparam name="T">The type of resource</typeparam>
    /// <param name="id">Id of resource</param>
    /// <param name="cancellationToken">Cancellation token for the request; not utilized if data has been cached</param>
    /// <returns>The object of the resource</returns>
    public async Task<T> GetResourceAsync<T>(int id, CancellationToken cancellationToken)
        where T : ResourceBase
    {
        return await GetResourcesWithParamsAsync<T>(id.ToString(), cancellationToken);
    }

    /// <summary>
    /// Gets a resource by name. This lookup
    /// is case insensitive.
    /// </summary>
    /// <typeparam name="T">The type of resource</typeparam>
    /// <param name="name">Name of resource</param>
    /// <returns>The object of the resource</returns>
    public async Task<T> GetResourceAsync<T>(string name)
        where T : NamedApiResource
    {
        return await GetResourceAsync<T>(name, CancellationToken.None);
    }

    /// <summary>
    /// Gets a resource by name. This lookup
    /// is case insensitive.
    /// </summary>
    /// <typeparam name="T">The type of resource</typeparam>
    /// <param name="name">Name of resource</param>
    /// <param name="cancellationToken">Cancellation token for the request; not utilized if data has been cached</param>
    /// <returns>The object of the resource</returns>
    public async Task<T> GetResourceAsync<T>(string name, CancellationToken cancellationToken)
        where T : NamedApiResource
    {
        string sanitizedName = name
            .Replace(" ", "-") // no resource can have a space in the name; API uses -'s in their place
            .Replace("'", "") // looking at you, Farfetch'd
            .Replace(".", ""); // looking at you, Mime Jr. and Mr. Mime

        // Nidoran is interesting as the API wants 'nidoran-f' or 'nidoran-m'

        return await GetResourcesWithParamsAsync<T>(sanitizedName, cancellationToken);
    }

    /// <summary>
    /// Resolves all navigation properties in a collection
    /// </summary>
    /// <typeparam name="T">Navigation type</typeparam>
    /// <param name="collection">The collection of navigation objects</param>
    /// <returns>A list of resolved objects</returns>
    public async Task<List<T>> GetResourceAsync<T>(IEnumerable<UrlNavigation<T>> collection)
        where T : ResourceBase
    {
        return await GetResourceAsync<T>(collection, CancellationToken.None);
    }

    /// <summary>
    /// Resolves all navigation properties in a collection
    /// </summary>
    /// <typeparam name="T">Navigation type</typeparam>
    /// <param name="collection">The collection of navigation objects</param>
    /// <param name="cancellationToken">Cancellation token for the request; not utilized if data has been cached</param>
    /// <returns>A list of resolved objects</returns>
    public async Task<List<T>> GetResourceAsync<T>(IEnumerable<UrlNavigation<T>> collection,
        CancellationToken cancellationToken)
        where T : ResourceBase
    {
        return (await Task.WhenAll(collection.Select(m => GetResourceAsync(m, cancellationToken)))).ToList();
    }

    /// <summary>
    /// Resolves a single navigation property
    /// </summary>
    /// <typeparam name="T">Navigation type</typeparam>
    /// <param name="urlResource">The single navigation object to resolve</param>
    /// <returns>A resolved object</returns>
    public async Task<T> GetResourceAsync<T>(UrlNavigation<T> urlResource)
        where T : ResourceBase
    {
        return await GetResourceByUrlAsync<T>(urlResource.Url, CancellationToken.None);
    }

    /// <summary>
    /// Resolves a single navigation property
    /// </summary>
    /// <typeparam name="T">Navigation type</typeparam>
    /// <param name="urlResource">The single navigation object to resolve</param>
    /// <param name="cancellationToken">Cancellation token for the request; not utilized if data has been cached</param>
    /// <returns>A resolved object</returns>
    public async Task<T> GetResourceAsync<T>(UrlNavigation<T> urlResource, CancellationToken cancellationToken)
        where T : ResourceBase
    {
        return await GetResourceByUrlAsync<T>(urlResource.Url, cancellationToken);
    }

    /// <summary>
    /// Handles all outbound API requests to the PokeAPI server and deserializes the response
    /// </summary>
    private async Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        using var response =
            await _client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);

        response.EnsureSuccessStatusCode();
        return DeserializeStream<T>(await response.Content.ReadAsStreamAsync());
    }

    /// <summary>
    /// Handles deserialization of a given stream to a given type
    /// </summary>
    private T? DeserializeStream<T>(Stream stream)
    {
        using var sr = new System.IO.StreamReader(stream);
        return JsonSerializer.Deserialize<T>(stream,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });
    }

    private static string GetApiEndpointString<T>()
    {
        PropertyInfo propertyInfo = typeof(T).GetProperty("ApiEndpoint", BindingFlags.Static | BindingFlags.NonPublic);
        return propertyInfo.GetValue(null).ToString();
    }

    private static bool IsApiEndpointCaseSensitive<T>()
    {
        PropertyInfo propertyInfo =
            typeof(T).GetProperty("IsApiEndpointCaseSensitive", BindingFlags.Static | BindingFlags.NonPublic);
        return propertyInfo == null ? false : (bool)propertyInfo.GetValue(null);
    }
}