// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace PokeQuiz.Models.PokeQuiz;

/// <summary>
/// A match up consisting of two <see cref="Pokemon"/> and a <see cref="Move"/>.
/// Represents a round in a Pokémon fight.
/// </summary>
[ExcludeFromCodeCoverage]
public class Matchup
{
    /// <summary>
    /// The attacking <see cref="Pokemon"/>.
    /// </summary>
    public List<Pokemon> Team { get; init; }

    /// <summary>
    /// The attacking <see cref="Pokemon"/>.
    /// </summary>
    public Pokemon Attacker { get; init; }

    /// <summary>
    /// The opposing <see cref="Pokemon"/>.
    /// </summary>
    public Pokemon Opponent { get; init; }

    /// <summary>
    /// The <see cref="Move"/> of the attacking Pokemon.
    /// </summary>
    public Move Move { get; init; }

    /// <summary>
    /// The <see cref="TypeEffectiveness">Guess</see> of the User.
    /// </summary>
    public TypeEffectiveness? Guess { get; init; }

    /// <summary>
    /// The <see cref="TypeEffectiveness"/> of the move against the defending Pokémon.
    /// </summary>
    public TypeEffectiveness Effectiveness { get; init; }
};