// ReSharper disable UnusedAutoPropertyAccessor.Global

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace PokeQuiz.Models.PokeQuiz;

/// <summary>
/// A match up consisting of two <see cref="Pokemon"/> and a <see cref="Move"/>.
/// Represents a round in a Pokémon fight.
/// </summary>
public class Matchup
{
    /// <summary>
    /// The attacking <see cref="Pokemon"/>.
    /// </summary>
    public Pokemon Attacker { get; init; }

    /// <summary>
    /// The defending <see cref="Pokemon"/>.
    /// </summary>
    public Pokemon Defender { get; init; }

    /// <summary>
    /// The <see cref="Move"/>, the attacking Pokémon uses.
    /// </summary>
    public Move Move { get; init; }

    /// <summary>
    /// The <see cref="TypeEffectiveness"/> of the move against the defending Pokémon.
    /// </summary>
    public TypeEffectiveness Effectiveness { get; init; }
};