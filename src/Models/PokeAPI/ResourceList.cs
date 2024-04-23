using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace PokeQuiz.Models.PokeApi;

/// <summary>
/// The base class for all resource lists
/// </summary>
[ExcludeFromCodeCoverage]
public abstract class ResourceList<T> where T : ResourceBase
{
    /// <summary>
    /// The total number of resources available from this API
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>
    /// The URL for the next page in the list.
    /// </summary>
    [JsonPropertyName("next")]
    public string? Next { get; set; }

    /// <summary>
    /// The URL for the previous page in the list.
    /// </summary>
    [JsonPropertyName("previous")]
    public string? Previous { get; set; }
}

/// <summary>
/// The paging object for un-named resources
/// </summary>
/// <typeparam name="T">The type of the paged resource</typeparam>
[ExcludeFromCodeCoverage]
public class ApiResourceList<T> : ResourceList<T> where T : ApiResource
{
    /// <summary>
    /// A list of un-named API resources.
    /// </summary>
    [JsonPropertyName("results")]
    public List<ApiResource<T>> Results { get; set; }
}

/// <summary>
/// The paging object for named resources
/// </summary>
/// <typeparam name="T">The type of the paged resource</typeparam>
[ExcludeFromCodeCoverage]
public class NamedApiResourceList<T> : ResourceList<T> where T : NamedApiResource
{
    /// <summary>
    /// A list of named API resources.
    /// </summary>
    [JsonPropertyName("results")]
    public List<NamedApiResource<T>> Results { get; set; }
}