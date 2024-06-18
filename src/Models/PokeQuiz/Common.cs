// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.Diagnostics.CodeAnalysis;
using PokeQuiz.Extensions;
using PokeQuiz.Models.PokeApi;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace PokeQuiz.Models.PokeQuiz;

/// <summary>
/// A name with language information
/// </summary>
[ExcludeFromCodeCoverage]
public class InternationalName : IDeserializable<Names>
{
    /// <summary>
    /// The localized name for an API resource in a specific language.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The language this name is in.
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// Maps a PokeApi.Names to a PokeQuiz.InternationalName
    /// </summary>
    /// <param name="names">The PokeApi.Names</param>
    /// <returns>The PokeQuiz.Type</returns>
    public void FromPokeApiResource(Names names)
    {
        Name = names.Name;
        Language = names.Language.Name;
    }
}