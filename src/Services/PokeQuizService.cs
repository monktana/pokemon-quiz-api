using System.Reflection;
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
    private readonly PokeApiRestClient _restClient = new(httpClient);

    /// <summary>
    /// Constructs a <see cref="PokeQuizModels.Matchup"/>.
    /// </summary>
    /// <returns>The matchup containing two <see cref="PokeQuizModels.Pokemon"/> and a <see cref="PokeQuizModels.Move"/></returns>
    public async Task<PokeQuizModels.Matchup> GetMatchup()
    {
        var team = await GetPokemonTeam();
        var attacker = team[new Random().Next(team.Count)];
        var opponent = await GetPokemon((new Random().Next(151) + 1).ToString());

        var move = GetRandomAttackingMove(attacker);
        var effectiveness = typeService.CalculateEffectiveness(move.Type, opponent.Types);

        return new PokeQuizModels.Matchup
        {
            Team = team,
            Attacker = attacker,
            Opponent = opponent,
            Move = move,
            Guess = null,
            Effectiveness = effectiveness
        };
    }

    /// <summary>
    /// Get a <see cref="PokeQuizModels.Pokemon">Pokemon</see> by name.
    /// </summary>
    /// <param name="name">The name of the <see cref="PokeQuizModels.Pokemon">Pokemon</see></param>
    /// <returns>The object representing <see cref="PokeQuizModels.Pokemon">Pokemon</see></returns>
    public async Task<PokeQuizModels.Pokemon> GetPokemon(string name)
    {
        var pokemon = await _restClient.GetResourceAsync<PokeAPIModels.Pokemon>(name);
        (Task<PokeQuizModels.PokemonSpecies> species, Task<List<PokeQuizModels.Move>> moves, Task<List<PokeQuizModels.Type>> types) tasks =
        (
            GetSpecies(pokemon.Species),
            GetMoves(pokemon.Moves.Select(move => move.Move)),
            GetTypes(pokemon.Types.Select(type => type.Type))
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
        var pokeApiSpecies = await _restClient.GetResourceAsync<PokeAPIModels.PokemonSpecies>(name);
        var species = new PokeQuizModels.PokemonSpecies();
        species.FromPokeApiResource(pokeApiSpecies);
        return species;
    }

    /// <summary>
    /// Get a <see cref="PokeQuizModels.Type"/> by name
    /// </summary>
    /// <param name="name">The name of the <see cref="PokeQuizModels.Type"/></param>
    /// <returns>The object representing the <see cref="PokeQuizModels.Type"/></returns>
    public async Task<PokeQuizModels.Type> GetType(string name)
    {
        var pokeApiType = await _restClient.GetResourceAsync<PokeAPIModels.Type>(name);
        var type = new PokeQuizModels.Type();
        type.FromPokeApiResource(pokeApiType);
        return type;
    }

    /// <summary>
    /// Get an overview of all <see cref="PokeQuizModels.Type"/>
    /// </summary>
    /// <returns>An overview of all available <see cref="PokeQuizModels.Type"/></returns>
    public Task<List<PokeAPIModels.NamedApiResource<PokeAPIModels.Type>>> GetTypes()
    {
        var response = new List<PokeAPIModels.NamedApiResource<PokeAPIModels.Type>>();
        var types = typeof(PokeQuizModels.Types);

        foreach (var property in types.GetFields(BindingFlags.Static | BindingFlags.Public))
        {
            var value = property.GetValue(null);
            response.Add(new PokeAPIModels.NamedApiResource<PokeAPIModels.Type>
            {
                Name = value?.ToString() ?? string.Empty,
                Url = string.Empty
            });
        }

        return Task.FromResult(response);
    }

    /// <summary>
    /// Get a <see cref="PokeQuizModels.Move"/> by name
    /// </summary>
    /// <param name="name">The name of the <see cref="PokeQuizModels.Move"/></param>
    /// <returns>The object representing the <see cref="PokeQuizModels.Move"/></returns>
    public async Task<PokeQuizModels.Move> GetMove(string name)
    {
        var pokeApiMove = await _restClient.GetResourceAsync<PokeAPIModels.Move>(name);
        var move = new PokeQuizModels.Move();
        move.FromPokeApiResource(pokeApiMove);

        var pokeApiType = await _restClient.GetResourceAsync(pokeApiMove.Type);
        var type = new PokeQuizModels.Type();
        type.FromPokeApiResource(pokeApiType);

        move.Type = type;

        return move;
    }

    /// <summary>
    /// Get a team of <see cref="PokeQuizModels.Pokemon">Pokemon</see>.
    /// </summary>
    /// <param name="size">The size of the team</param>
    /// <returns>A list of <see cref="PokeQuizModels.Pokemon">Pokemon</see></returns>
    private async Task<List<PokeQuizModels.Pokemon>> GetPokemonTeam(int size = 6)
    {
        var tasks = new List<Task<PokeQuizModels.Pokemon>>();
        for (var i = 0; i < size; i++)
        {
            tasks.Add(GetPokemon((new Random().Next(151) + 1).ToString()));
        }

        return (await Task.WhenAll(tasks)).ToList();
    }

    /// <summary>
    /// Get a random attacking <see cref="PokeQuizModels.Move"/> learned by the provided <see cref="PokeQuizModels.Pokemon"/>
    /// </summary>
    /// <param name="pokemon">The <see cref="PokeQuizModels.Pokemon"/> to get the move off</param>
    /// <returns>The object representing the <see cref="PokeQuizModels.Move"/></returns>
    private static PokeQuizModels.Move GetRandomAttackingMove(PokeQuizModels.Pokemon pokemon)
    {
        var attackingMoves = pokemon.Moves.FindAll(move => move.Power > 0).ToArray();
        var random = new Random().Next(attackingMoves.Length);

        return attackingMoves[random];
    }

    /// <summary>
    /// Get a <see cref="PokeQuizModels.PokemonSpecies"/> by url.
    /// </summary>
    /// <param name="url">The resource url of the <see cref="PokeQuizModels.PokemonSpecies"/></param>
    /// <returns>The object representing the <see cref="PokeQuizModels.PokemonSpecies"/></returns>
    public async Task<PokeQuizModels.PokemonSpecies> GetSpecies(PokeAPIModels.UrlNavigation<PokeAPIModels.PokemonSpecies> url)
    {
        var pokeApiSpecies = await _restClient.GetResourceAsync(url);
        var species = new PokeQuizModels.PokemonSpecies();
        species.FromPokeApiResource(pokeApiSpecies);
        return species;
    }

    /// <summary>
    /// Get a <see cref="PokeQuizModels.Type"/> by url
    /// </summary>
    /// <param name="url">The resource url of the <see cref="PokeQuizModels.Type"/></param>
    /// <returns>The object representing the <see cref="PokeQuizModels.Type"/></returns>
    public async Task<PokeQuizModels.Type> GetType(PokeAPIModels.UrlNavigation<PokeAPIModels.Type> url)
    {
        var pokeApiType = await _restClient.GetResourceAsync(url);
        var type = new PokeQuizModels.Type();
        type.FromPokeApiResource(pokeApiType);
        return type;
    }

    /// <summary>
    /// Get a list of <see cref="PokeQuizModels.Type"/> by name
    /// </summary>
    /// <param name="typeNames">The list of <see cref="PokeQuizModels.Type"/> names</param>
    /// <returns>A list of objects representing <see cref="PokeQuizModels.Type"/>s</returns>
    public async Task<List<PokeQuizModels.Type>> GetTypes(IEnumerable<string> typeNames)
    {
        return (await Task.WhenAll(typeNames.Select(GetType))).ToList();
    }

    /// <summary>
    /// Get a list of <see cref="PokeQuizModels.Type"/> by their urls
    /// </summary>
    /// <param name="urls">The list of <see cref="PokeQuizModels.Type"/> urls</param>
    /// <returns>A list of objects representing <see cref="PokeQuizModels.Type"/>s</returns>
    private async Task<List<PokeQuizModels.Type>> GetTypes(IEnumerable<PokeAPIModels.UrlNavigation<PokeAPIModels.Type>> urls)
    {
        return (await Task.WhenAll(urls.Select(GetType))).ToList();
    }

    /// <summary>
    /// Get a <see cref="PokeQuizModels.Move"/> by url
    /// </summary>
    /// <param name="url">The resource url of the <see cref="PokeQuizModels.Move"/></param>
    /// <returns>The object representing the <see cref="PokeQuizModels.Move"/></returns>
    public async Task<PokeQuizModels.Move> GetMove(PokeAPIModels.UrlNavigation<PokeAPIModels.Move> url)
    {
        var pokeApiMove = await _restClient.GetResourceAsync(url);
        var move = new PokeQuizModels.Move();
        move.FromPokeApiResource(pokeApiMove);

        var type = await GetType(pokeApiMove.Type);
        move.Type = type;

        return move;
    }

    /// <summary>
    /// Get a list of <see cref="PokeQuizModels.Move"/> by name
    /// </summary>
    /// <param name="moveNames">The list of <see cref="PokeQuizModels.Move"/> names</param>
    /// <returns>A list of objects representing <see cref="PokeQuizModels.Move"/>s</returns>
    public async Task<List<PokeQuizModels.Move>> GetMoves(IEnumerable<string> moveNames)
    {
        var list = new List<Task<PokeQuizModels.Move>>();
        foreach (var moveName in moveNames)
        {
            var move = _restClient.GetResourceAsync<PokeAPIModels.Move>(moveName).Result;
            var type = GetType(move.Type).Result;

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
    /// Get a list of <see cref="PokeQuizModels.Move"/> by their urls
    /// </summary>
    /// <param name="urls">The list of <see cref="PokeQuizModels.Move"/> urls</param>
    /// <returns>A list of objects representing <see cref="PokeQuizModels.Move"/>s</returns>
    private async Task<List<PokeQuizModels.Move>> GetMoves(IEnumerable<PokeAPIModels.UrlNavigation<PokeAPIModels.Move>> urls)
    {
        var list = new List<Task<PokeQuizModels.Move>>();
        foreach (var url in urls)
        {
            var move = _restClient.GetResourceAsync(url).Result;
            var type = GetType(move.Type).Result;

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
}