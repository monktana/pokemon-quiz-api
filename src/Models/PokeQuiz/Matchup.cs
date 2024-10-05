// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace PokeQuiz.Models.PokeQuiz;

/// <summary>
/// A member of the <see cref="Pokemon"/> team used in a <see cref="Matchup"/>.
/// Contains the <see cref="Pokemon"/> itself and it's current state.
/// </summary>
[ExcludeFromCodeCoverage]
public class TeamMember
{
    /// <summary>
    /// The attacking <see cref="Pokemon"/>.
    /// </summary>
    public Pokemon Pokemon { get; init; }

    /// <summary>
    /// The opposing <see cref="Pokemon"/>.
    /// </summary>
    public bool Fainted { get; set; }

    /// <summary>
    /// Check if TeamMember is fainted.
    /// </summary>
    public bool IsFainted => Fainted;
};

/// <summary>
/// A match up consisting of two <see cref="Pokemon"/> and a <see cref="Move"/>.
/// Represents a round in a Pokémon fight.
/// </summary>
[ExcludeFromCodeCoverage]
[DebuggerDisplay("Attacker: {Attacker.Name}, Opponent: {Opponent.Name}, Move: {Move.Name}")]
public record struct Matchup
{
    /// <summary>
    /// The attacking <see cref="Pokemon"/>.
    /// </summary>
    public List<TeamMember> Team { get; init; }

    /// <summary>
    /// The attacking <see cref="Pokemon"/>.
    /// </summary>
    public Pokemon Attacker { get; set; }

    /// <summary>
    /// The opposing <see cref="Pokemon"/>.
    /// </summary>
    public Pokemon Opponent { get; init; }

    /// <summary>
    /// The <see cref="Move"/> of the attacking Pokemon.
    /// </summary>
    public Move Move { get; set; }

    /// <summary>
    /// The <see cref="TypeEffectiveness">Guess</see> of the User.
    /// </summary>
    public TypeEffectiveness? Guess { get; set; }

    /// <summary>
    /// The <see cref="TypeEffectiveness"/> of the move against the defending Pokémon.
    /// </summary>
    public TypeEffectiveness Effectiveness { get; set; }
};