using System.Diagnostics;
using PokeQuiz.Models.PokeQuiz;

namespace PokeQuiz.Services;

/// <summary>
/// 
/// </summary>
public class PokeQuizService(HttpClient httpClient)
{
    private readonly PokeApiClient _client = new(httpClient);

    public async Task<Matchup> GetMatchup()
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        (Task<Pokemon> attacker, Task<Pokemon> defender) tasks = (
            GetPokemon((new Random()).Next(151) + 1),
            GetPokemon((new Random()).Next(151) + 1)
        );
        await Task.WhenAll(tasks.attacker, tasks.defender);
        stopWatch.Stop();

        Console.WriteLine("Both Pokemon: " + stopWatch.Elapsed);

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
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        var pokemon = await _client.GetResourceAsync<Models.PokeApi.Pokemon>(id);
        (Task<PokemonSpecies> species, Task<List<Move>> moves, Task<List<PokeQuiz.Models.PokeQuiz.Type>> types) tasks =
        (
            GetSpecies(pokemon),
            GetMoves(pokemon),
            GetTypes(pokemon)
        );

        await Task.WhenAll(tasks.species, tasks.moves, tasks.types);
        var (species, moves, types) = tasks;

        var response = new Pokemon
        {
            Id = pokemon.Id,
            Name = pokemon.Name,
            Species = species.Result,
            Moves = moves.Result,
            Types = types.Result
        };
        stopWatch.Stop();

        Console.WriteLine($"Single Pokemon ({id}): " + stopWatch.Elapsed);

        return response;
    }

    private async Task<PokemonSpecies> GetSpecies(Models.PokeApi.Pokemon pokemon)
    {
        return PokemonSpecies.FromPokeApiResource(await _client.GetResourceAsync(pokemon.Species));
    }

    private async Task<List<Models.PokeQuiz.Type>> GetTypes(Models.PokeApi.Pokemon pokemon)
    {
        return (await _client.GetResourceAsync(pokemon.Types.Select(type => type.Type)))
            .Select(Models.PokeQuiz.Type.FromPokeApiResource).ToList();
    }

    private async Task<List<Move>> GetMoves(Models.PokeApi.Pokemon pokemon)
    {
        var list = (from move in pokemon.Moves
            select _client.GetResourceAsync(move.Move).Result
            into fullMove
            let moveType = _client.GetResourceAsync(fullMove.Type).Result
            select Task.FromResult(new Move
            {
                Id = fullMove.Id,
                Name = fullMove.Name,
                Names = fullMove.Names.Select(InternationalName.FromPokeApiResource).ToList(),
                Power = fullMove.Power,
                Type = Models.PokeQuiz.Type.FromPokeApiResource(moveType),
            })).ToList();

        return (await Task.WhenAll(list)).ToList();
    }

    private static Move GetMove(Pokemon pokemon)
    {
        var attackingMoves = pokemon.Moves.FindAll(move => move.Power > 0).ToArray();
        var random = (new Random()).Next(attackingMoves.Length);
        return attackingMoves[random];
    }
}