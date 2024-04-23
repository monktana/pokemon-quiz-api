using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace PokeQuiz.Models.PokeApi;

/// <summary>
/// Allows for automatic fetching of a resource via a url
/// </summary>
[ExcludeFromCodeCoverage]
public abstract record UrlNavigation<T> where T : ResourceBase
{
    /// <summary>
    /// Url of the referenced resource
    /// </summary>
    public string Url { get; set; }
}