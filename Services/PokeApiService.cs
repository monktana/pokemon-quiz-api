using System.Diagnostics;
using PokeApiNet;
using PokeQuiz.Models;
using Move = PokeQuiz.Models.Move;
using Pokemon = PokeQuiz.Models.Pokemon;
using PokemonSpecies = PokeQuiz.Models.PokemonSpecies;
using Type = PokeQuiz.Models.Type;

namespace PokeQuiz.Services;

/// <summary>
/// 
/// </summary>
public class PokeApiService(HttpClient httpClient)
{
  private readonly PokeApiClient _client = new(httpClient);

  public async Task<Matchup> GetMatchup()
  {
    (Task<Pokemon> attacker, Task<Pokemon> defender) tasks = (
      GetPokemon(3),
      GetPokemon(6)
    );
    await Task.WhenAll(tasks.attacker, tasks.defender);
    
    var (attacker, defender) = tasks;

    var move = GetMove(attacker.Result);

    return new Matchup
    {
      Attacker = attacker.Result,
      Defender = defender.Result,
      Move = move
    };
  }

  private async Task<Pokemon> GetPokemon(int id)
  {
    var stopwatch = Stopwatch.StartNew();
    
    var pokemon = await _client.GetResourceAsync<PokeApiNet.Pokemon>(id);
    (Task<PokemonSpecies> species, Task<List<Move>> moves, Task<List<Type>> types) tasks = (
      GetSpecies(pokemon),
      GetMoves(pokemon),
      GetTypes(pokemon)
    );
    
    await Task.WhenAll(tasks.species, tasks.moves, tasks.types);
    var (species, moves, types) = tasks;
    
    stopwatch.Stop();
    Console.WriteLine($"Single Pokemon({id}): {stopwatch.Elapsed}");

    var response = new Pokemon
    {
      Id = pokemon.Id,
      Name = pokemon.Name,
      Species = species.Result,
      Moves = moves.Result,
      Types = types.Result
    };
    
    return response;
  }
  
  private async Task<PokemonSpecies> GetSpecies(PokeApiNet.Pokemon pokemon)
  {
    return PokemonSpecies.FromPokeApiNetResource(await _client.GetResourceAsync(pokemon.Species));
  }
  
  private async Task<List<Type>> GetTypes(PokeApiNet.Pokemon pokemon)
  {
    return (await _client.GetResourceAsync(pokemon.Types.Select(type => type.Type))).Select(Type.FromPokeApiNetResource).ToList();
  }

  private async Task<List<Move>> GetMoves(PokeApiNet.Pokemon pokemon)
  {
    var list = new List<Task<Move>>();
    foreach (var move in pokemon.Moves)
    {
      var fullMove = _client.GetResourceAsync(move.Move).Result;
      var moveType = _client.GetResourceAsync(fullMove.Type).Result;

      list.Add(Task.FromResult(new Move
      {
        Id = fullMove.Id,
        Name = fullMove.Name,
        Names = Enumerable.ToList(Enumerable.Select(fullMove.Names, InternationalName.FromPokeApiNetResource)),
        Power = fullMove.Power,
        Type = Type.FromPokeApiNetResource(moveType),
      }));
    }

    return (await Task.WhenAll(list)).ToList();
  }

  private static Move GetMove(Pokemon pokemon)
  {
    return pokemon.Moves.FindAll(move => move.Power > 0).ToList().First();
  }
}