// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace PokeQuiz.Models;

public class Move
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
  /// Maps a PokeApiNet.PokemonSpecies to a PokeQuiz.PokemonSpecies
  /// </summary>
  /// <param name="move">The PokeApiNet.Move</param>
  /// <returns>The PokeQuiz.PokemonSpecies</returns>
  public static Move FromPokeApiNetResource(PokeApiNet.Move move)
  {
    return new Move
    {
      Id = move.Id,
      Name = move.Name,
      Names = move.Names.Select(name => new InternationalName { Name = name.Name, Language = name.Language.Name }).ToList(),
      Power = move.Power,
    };
  }
}