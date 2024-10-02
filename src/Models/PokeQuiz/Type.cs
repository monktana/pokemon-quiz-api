using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using PokeQuiz.Extensions;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace PokeQuiz.Models.PokeQuiz;

/// <summary>
/// Types are properties for Pokémon and their moves. Each type can be
/// super effective, effective, not very effective or completely ineffective against#
/// another type.
/// <remarks>The different type effectiveness are represented by <see cref="TypeEffectiveness"/></remarks>
/// </summary>
[ExcludeFromCodeCoverage]
[DebuggerDisplay("{Name} ({Id})")]
public record struct Type : IDeserializable<PokeApi.Type>
{
    /// <summary>
    /// The identifier for this resource.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name for this resource.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The name of this resource listed in different languages.
    /// </summary>
    public List<InternationalName> Names { get; set; }

    /// <summary>
    /// Maps a PokeApi.Type to a PokeQuiz.Type
    /// </summary>
    /// <param name="type">The PokeApi.Type</param>
    /// <returns>The PokeQuiz.Type</returns>
    public void FromPokeApiResource(PokeApi.Type type)
    {
        Id = type.Id;
        Name = type.Name;
        Names = type.Names.Select(name =>
        {
            var internationalName = new InternationalName();
            internationalName.FromPokeApiResource(name);
            return internationalName;
        }).ToList();
    }
}

/// <summary>
/// Representation of all available types
/// </summary>
public enum Types
{
    Bug,
    Dark,
    Dragon,
    Electric,
    Fairy,
    Fighting,
    Fire,
    Flying,
    Ghost,
    Grass,
    Ground,
    Ice,
    Normal,
    Poison,
    Psychic,
    Rock,
    Steel,
    Water,
}

/// <summary>
/// Representation of the different type match ups.
/// </summary>
public enum TypeEffectiveness
{
    NoEffect,
    NotVeryEffective,
    Effective,
    SuperEffective
}