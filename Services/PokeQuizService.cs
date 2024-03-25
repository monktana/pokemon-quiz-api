using PokeQuiz.Models.PokeQuiz;
using Move = PokeQuiz.Models.PokeQuiz.Move;
using Pokemon = PokeQuiz.Models.PokeQuiz.Pokemon;
using PokemonSpecies = PokeQuiz.Models.PokeQuiz.PokemonSpecies;
using Type = PokeQuiz.Models.PokeApi.Type;

namespace PokeQuiz.Services;

/// <summary>
/// Gets data from PokeAPI and transforms it to the PokeQuiz models.
/// </summary>
/// <param name="httpClient">HttpClient implementation</param>
public class PokeQuizService(HttpClient httpClient)
{
    private readonly PokeApiClient _client = new(httpClient);

    /// <summary>
    /// Constructs a <see cref="Matchup"/>.
    /// </summary>
    /// <returns>The matchup containing two <see cref="Pokemon"/> and a <see cref="Move"/></returns>
    public async Task<Matchup> GetMatchup()
    {
        (Task<Pokemon> attacker, Task<Pokemon> defender) tasks = (
            GetPokemon((new Random()).Next(151) + 1),
            GetPokemon((new Random()).Next(151) + 1)
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

    /// <summary>
    /// Gets a <see cref="Pokemon"/> by id
    /// </summary>
    /// <param name="id">The id of the <see cref="Pokemon"/></param>
    /// <returns>The object representing <see cref="Pokemon"/></returns>
    public async Task<Pokemon> GetPokemon(int id)
    {
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

        return response;
    }

    /// <summary>
    /// Gets the <see cref="PokemonSpecies"/> of the <see cref="Pokemon"/>.
    /// </summary>
    /// <param name="pokemon">The <see cref="Pokemon"/> to get the <see cref="PokemonSpecies"/> of</param>
    /// <returns>The object representing the <see cref="PokemonSpecies"/></returns>
    public async Task<PokemonSpecies> GetSpecies(Models.PokeApi.Pokemon pokemon)
    {
        return PokemonSpecies.FromPokeApiResource(await _client.GetResourceAsync(pokemon.Species));
    }

    /// <summary>
    /// Gets a List of all <see cref="Models.PokeQuiz.Type"/> of the <see cref="Pokemon"/>
    /// </summary>
    /// <param name="pokemon">The <see cref="Pokemon"/> to get the <see cref="Models.PokeQuiz.Type"/> of</param>
    /// <returns>A list of objects representing the <see cref="Models.PokeQuiz.Type"/></returns>
    public async Task<List<Models.PokeQuiz.Type>> GetTypes(Models.PokeApi.Pokemon pokemon)
    {
        return (await _client.GetResourceAsync(pokemon.Types.Select(type => type.Type)))
            .Select(Models.PokeQuiz.Type.FromPokeApiResource).ToList();
    }

    /// <summary>
    /// Gets a List of all <see cref="Models.PokeQuiz.Move"/> of the <see cref="Pokemon"/>
    /// </summary>
    /// <param name="pokemon">The <see cref="Pokemon"/> to get the <see cref="Models.PokeQuiz.Move"/> of</param>
    /// <returns>A list of objects representing the <see cref="Models.PokeQuiz.Move"/></returns>
    public async Task<List<Move>> GetMoves(Models.PokeApi.Pokemon pokemon)
    {
        var list = new List<Task<Move>>();
        foreach (var move in pokemon.Moves)
        {
            var fullMove = _client.GetResourceAsync(move.Move).Result;
            Type moveType = _client.GetResourceAsync(fullMove.Type).Result;
            list.Add(Task.FromResult(new Move
            {
                Id = fullMove.Id,
                Name = fullMove.Name,
                Names = fullMove.Names.Select(InternationalName.FromPokeApiResource).ToList(),
                Power = fullMove.Power,
                Type = Models.PokeQuiz.Type.FromPokeApiResource(moveType),
            }));
        }

        return (await Task.WhenAll(list)).ToList();
    }

    private static Move GetMove(Pokemon pokemon)
    {
        var attackingMoves = pokemon.Moves.FindAll(move => move.Power > 0).ToArray();
        var random = (new Random()).Next(attackingMoves.Length);
        return attackingMoves[random];
    }
}