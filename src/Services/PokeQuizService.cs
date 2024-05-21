using PokeQuiz.Clients;
using PokeQuizModels = PokeQuiz.Models.PokeQuiz;
using PokeAPIModels = PokeQuiz.Models.PokeApi;

namespace PokeQuiz.Services;

/// <summary>
/// Gets data from PokeAPI and transforms it to the PokeQuiz models.
/// </summary>
/// <param name="httpClient">HttpClient implementation</param>
public class PokeQuizService(HttpClient httpClient, TypeEffectivenessService typeService) : IPokeQuizService
{
    private readonly PokeApiClient _client = new(httpClient);

    /// <summary>
    /// Constructs a <see cref="PokeQuizModels.Matchup"/>.
    /// </summary>
    /// <returns>The matchup containing two <see cref="PokeQuizModels.Pokemon"/> and a <see cref="PokeQuizModels.Move"/></returns>
    public async Task<PokeQuizModels.Matchup> GetMatchup()
    {
        (Task<PokeQuizModels.Pokemon> attacker, Task<PokeQuizModels.Pokemon> defender) tasks = (
            GetPokemon((new Random().Next(151) + 1).ToString()),
            GetPokemon((new Random().Next(151) + 1).ToString())
        );
        await Task.WhenAll(tasks.attacker, tasks.defender);

        var (attacker, defender) = tasks;

        var move = GetRandomAttackingMove(attacker.Result);
        var effectiveness = typeService.CalculateEffectiveness(move.Type, defender.Result.Types);

        return new PokeQuizModels.Matchup
        {
            Attacker = attacker.Result,
            Defender = defender.Result,
            Move = move,
            Effectiveness = effectiveness
        };
    }

    /// <summary>
    /// Get a <see cref="PokeQuizModels.Pokemon"/> by name.
    /// </summary>
    /// <param name="name">The name of the <see cref="PokeQuizModels.Pokemon"/></param>
    /// <returns>The object representing <see cref="PokeQuizModels.Pokemon"/></returns>
    public async Task<PokeQuizModels.Pokemon> GetPokemon(string name)
    {
        var pokemon = await _client.GetResourceAsync<PokeAPIModels.Pokemon>(name);
        (Task<PokeQuizModels.PokemonSpecies> species, Task<List<PokeQuizModels.Move>> moves, Task<List<PokeQuizModels.Type>> types) tasks =
        (
            GetSpecies(pokemon.Name),
            GetMoves(pokemon.Moves.Select(move => move.Move.Name)),
            GetTypes(pokemon.Types.Select(type => type.Type.Name))
        );

        await Task.WhenAll(tasks.species, tasks.moves, tasks.types);
        var (species, moves, types) = tasks;

        var sprites = new PokeQuizModels.PokemonSprites();
        sprites.FromPokeApiResource(pokemon.Sprites);

        return new PokeQuizModels.Pokemon
        {
            Id = pokemon.Id,
            Name = pokemon.Name,
            Species = species.Result,
            Moves = moves.Result,
            Types = types.Result,
            Sprites = sprites
        };
    }

    /// <summary>
    /// Get a <see cref="PokeQuizModels.PokemonSpecies"/> by name.
    /// </summary>
    /// <param name="name">The name of the <see cref="PokeQuizModels.PokemonSpecies"/></param>
    /// <returns>The object representing the <see cref="PokeQuizModels.PokemonSpecies"/></returns>
    public async Task<PokeQuizModels.PokemonSpecies> GetSpecies(string name)
    {
        var pokeApiSpecies = await _client.GetResourceAsync<PokeAPIModels.PokemonSpecies>(name);
        var species = new PokeQuizModels.PokemonSpecies();
        species.FromPokeApiResource(pokeApiSpecies);
        return species;
    }

    /// <summary>
    /// Get a <see cref="Models.PokeQuiz.Type"/> by name
    /// </summary>
    /// <param name="name">The name of the <see cref="Models.PokeQuiz.Type"/></param>
    /// <returns>The object representing the <see cref="Models.PokeQuiz.Type"/></returns>
    public async Task<PokeQuizModels.Type> GetType(string name)
    {
        var pokeApiType = await _client.GetResourceAsync<PokeAPIModels.Type>(name);
        var type = new PokeQuizModels.Type();
        type.FromPokeApiResource(pokeApiType);
        return type;
    }

    /// <summary>
    /// Get an overview of all <see cref="Models.PokeQuiz.Type"/>
    /// </summary>
    /// <returns>An overview of all available <see cref="Models.PokeQuiz.Type"/></returns>
    public Task<List<PokeAPIModels.NamedApiResource<PokeAPIModels.Type>>> GetTypes()
    {
        var response = new List<PokeAPIModels.NamedApiResource<PokeAPIModels.Type>>();
        var types = typeof(PokeQuizModels.Types);

        foreach (var property in types.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public))
        {
            var value = property.GetValue(null);
            response.Add(new PokeAPIModels.NamedApiResource<PokeAPIModels.Type>
            {
                Name = value.ToString(),
                Url = $"{Environment.GetEnvironmentVariable("ASPNETCORE_URLS")}/api/v1/type/{value}"
            });
        }

        return Task.FromResult(response);
    }

    /// <summary>
    /// Get a <see cref="Models.PokeQuiz.Move"/> by name
    /// </summary>
    /// <param name="name">The name of the <see cref="Models.PokeQuiz.Move"/></param>
    /// <returns>The object representing the <see cref="Models.PokeQuiz.Move"/></returns>
    public async Task<PokeQuizModels.Move> GetMove(string name)
    {
        var pokeApiMove = await _client.GetResourceAsync<PokeAPIModels.Move>(name);
        var move = new PokeQuizModels.Move();
        move.FromPokeApiResource(pokeApiMove);

        var pokeApiType = await _client.GetResourceAsync(pokeApiMove.Type);
        var type = new PokeQuizModels.Type();
        type.FromPokeApiResource(pokeApiType);

        move.Type = type;

        return move;
    }

    /// <summary>
    /// Get a list of <see cref="Models.PokeQuiz.Type"/> by name
    /// </summary>
    /// <param name="typeNames">The list of <see cref="Models.PokeQuiz.Type"/> names</param>
    /// <returns>A list of objects representing <see cref="Models.PokeQuiz.Type"/>s</returns>
    private async Task<List<PokeQuizModels.Type>> GetTypes(IEnumerable<string> typeNames)
    {
        return (await Task.WhenAll(typeNames.Select(GetType))).ToList();
    }

    /// <summary>
    /// Get a list of <see cref="Models.PokeQuiz.Move"/> by name
    /// </summary>
    /// <param name="moveNames">The list of <see cref="Models.PokeQuiz.Move"/> names</param>
    /// <returns>A list of objects representing <see cref="Models.PokeQuiz.Move"/>s</returns>
    private async Task<List<PokeQuizModels.Move>> GetMoves(IEnumerable<string> moveNames)
    {
        var list = new List<Task<PokeQuizModels.Move>>();
        foreach (var moveName in moveNames)
        {
            var move = _client.GetResourceAsync<PokeAPIModels.Move>(moveName).Result;
            var pokeApiType = _client.GetResourceAsync(move.Type).Result;
            var type = new PokeQuizModels.Type();
            type.FromPokeApiResource(pokeApiType);

            list.Add(Task.FromResult(new PokeQuizModels.Move
            {
                Id = move.Id,
                Name = move.Name,
                Names = move.Names.Select(name =>
                {
                    var internationalName = new PokeQuizModels.InternationalName();
                    internationalName.FromPokeApiResource(name);
                    return internationalName;
                }).ToList(),
                Power = move.Power,
                Type = type,
            }));
        }

        return (await Task.WhenAll(list)).ToList();
    }

    /// <summary>
    /// Get a random attacking <see cref="Models.PokeQuiz.Move"/> learned by the provided <see cref="PokeQuizModels.Pokemon"/>
    /// </summary>
    /// <param name="pokemon">The <see cref="PokeQuizModels.Pokemon"/> to get the move off</param>
    /// <returns>The object representing the <see cref="PokeQuizModels.Move"/></returns>
    private static PokeQuizModels.Move GetRandomAttackingMove(PokeQuizModels.Pokemon pokemon)
    {
        var attackingMoves = pokemon.Moves.FindAll(move => move.Power > 0).ToArray();
        var random = new Random().Next(attackingMoves.Length);
        return attackingMoves[random];
    }
}