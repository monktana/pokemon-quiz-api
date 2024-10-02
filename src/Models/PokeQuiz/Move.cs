// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using PokeQuiz.Extensions;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace PokeQuiz.Models.PokeQuiz;

[ExcludeFromCodeCoverage]
[DebuggerDisplay("{Name} ({Id})")]
public record struct Move : IDeserializable<PokeApi.Move>
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
    /// The base power of this move with a value of 0 if it does not have
    /// a base power.
    /// </summary>
    public int? Power { get; set; }

    /// <summary>
    /// The type of this move.
    /// </summary>
    public Type Type { get; set; }

    /// <summary>
    /// Checks if the move is an attacking move.
    /// </summary>
    public bool IsAttackingMove => Power > 0;

    /// <summary>
    /// Maps a PokeApi.PokemonSpecies to a PokeQuiz.PokemonSpecies
    /// </summary>
    /// <param name="move">The PokeApi.Move</param>
    /// <returns>The PokeQuiz.PokemonSpecies</returns>
    public void FromPokeApiResource(PokeApi.Move move)
    {
        Id = move.Id;
        Name = move.Name;
        Names = move.Names.Select(name =>
            {
                var internationalName = new InternationalName();
                internationalName.FromPokeApiResource(name);
                return internationalName;
            })
            .ToList();
        Power = move.Power ?? 0;
    }
}