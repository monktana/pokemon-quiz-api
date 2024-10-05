// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable ClassNeverInstantiated.Global

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using PokeQuiz.Extensions;

namespace PokeQuiz.Models.PokeQuiz;

/// <summary>
/// Pokémon sprite information
/// </summary>
[ExcludeFromCodeCoverage]
public record struct PokemonSprites : IDeserializable<PokeApi.PokemonSprites>
{
    /// <summary>
    /// The default depiction of this Pokémon from the front in battle.
    /// </summary>
    [JsonPropertyName("front_default")]
    public string FrontDefault { get; set; }

    /// <summary>
    /// The shiny depiction of this Pokémon from the front in battle.
    /// </summary>
    [JsonPropertyName("front_shiny")]
    public string FrontShiny { get; set; }

    /// <summary>
    /// The female depiction of this Pokémon from the front in battle.
    /// </summary>
    [JsonPropertyName("front_female")]
    public string FrontFemale { get; set; }

    /// <summary>
    /// The shiny female depiction of this Pokémon from the front in battle.
    /// </summary>
    [JsonPropertyName("front_shiny_female")]
    public string FrontShinyFemale { get; set; }

    /// <summary>
    /// The default depiction of this Pokémon from the back in battle.
    /// </summary>
    [JsonPropertyName("back_default")]
    public string BackDefault { get; set; }

    /// <summary>
    /// The shiny depiction of this Pokémon from the back in battle.
    /// </summary>
    [JsonPropertyName("back_shiny")]
    public string BackShiny { get; set; }

    /// <summary>
    /// The female depiction of this Pokémon from the back in battle.
    /// </summary>
    [JsonPropertyName("back_female")]
    public string BackFemale { get; set; }

    /// <summary>
    /// The shiny female depiction of this Pokémon from the back in battle.
    /// </summary>
    [JsonPropertyName("back_shiny_female")]
    public string BackShinyFemale { get; set; }

    /// <summary>
    /// Maps a <see cref="PokeApi.PokemonSprites"/> to a <see cref="PokemonSprites"/>
    /// </summary>
    /// <param name="sprites">The PokeApi.PokemonSprites</param>
    /// <returns>The PokeQuiz.PokemonSprites</returns>
    public void FromPokeApiResource(PokeApi.PokemonSprites sprites)
    {
        FrontDefault = sprites.FrontDefault;
        FrontShiny = sprites.FrontShiny;
        FrontFemale = sprites.FrontFemale;
        FrontShinyFemale = sprites.FrontShinyFemale;
        BackDefault = sprites.BackDefault;
        BackShiny = sprites.BackShiny;
        BackFemale = sprites.BackFemale;
        BackShinyFemale = sprites.BackShinyFemale;
    }
};

/// <summary>
/// A Pokémon Species forms the basis for at least one Pokémon. Attributes
/// of a Pokémon species are shared across all varieties of Pokémon within
/// the species. A good example is Wormadam; Wormadam is the species which
/// can be found in three different varieties, Wormadam-Trash,
/// Wormadam-Sandy and Wormadam-Plant.
/// </summary>
[ExcludeFromCodeCoverage]
public record struct PokemonSpecies : IDeserializable<PokeApi.PokemonSpecies>
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
    /// Maps a PokeApi.PokemonSpecies to a PokeQuiz.PokemonSpecies
    /// </summary>
    /// <param name="species">The PokeApi.Type</param>
    /// <returns>The PokeQuiz.PokemonSpecies</returns>
    public void FromPokeApiResource(PokeApi.PokemonSpecies species)
    {
        Id = species.Id;
        Name = species.Name;
        Names = species.Names.Select(name =>
        {
            var internationalName = new InternationalName();
            internationalName.FromPokeApiResource(name);
            return internationalName;
        }).ToList();
    }
}

/// <summary>
/// Pokémon are the creatures that inhabit the world of the Pokémon games.
/// They can be caught using Pokéballs and trained by battling with other Pokémon.
/// Each Pokémon belongs to a specific species but may take on a variant which
/// makes it differ from other Pokémon of the same species, such as base stats,
/// available abilities and typings.
/// </summary>
[ExcludeFromCodeCoverage]
[DebuggerDisplay("{Name} ({Id})")]
public record struct Pokemon
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
    /// A list of moves along with learn methods and level
    /// details pertaining to specific version groups.
    /// </summary>
    public List<Move> Moves { get; set; }

    /// <summary>
    /// A set of sprites used to depict this Pokémon in the game.
    /// </summary>
    public PokemonSprites Sprites { get; set; }

    /// <summary>
    /// The species this Pokémon belongs to.
    /// </summary>
    public PokemonSpecies Species { get; set; }

    /// <summary>
    /// A list of details showing types this Pokémon has.
    /// </summary>
    public List<Type> Types { get; set; }
}