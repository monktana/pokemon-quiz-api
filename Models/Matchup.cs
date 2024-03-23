// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace PokeQuiz;

/// <summary>
/// 
/// </summary>
public class Matchup
{
  /// <summary>
  /// The attacking Pokemon.
  /// </summary>
  public Pokemon Attacker { get; set; }
  
  /// <summary>
  /// The defending Pokemon.
  /// </summary>
  public Pokemon Defender { get; set; }

  /// <summary>
  /// The move, the attacking pokemon uses.
  /// </summary>
  public Move Move { get; set; }
};