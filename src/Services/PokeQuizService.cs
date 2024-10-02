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
    /// Creates an initial <see cref="PokeQuizModels.Matchup"/>.
    /// </summary>
    /// <returns>
    /// The matchup containing a <see cref="PokeQuizModels.TeamMember">List of Team member</see>, an <see cref="PokeQuizModels.Pokemon">Attacker</see> and <see cref="PokeQuizModels.Pokemon">Defender</see>,
    /// a <see cref="PokeQuizModels.Move">Move used by the Attacker</see> and an <see cref="PokeQuizModels.TypeEffectiveness">Effectiveness</see> of the move against the Defender
    /// </returns>
    public async Task<PokeQuizModels.Matchup> GetMatchup()
    {
        var team = await GetTeam();
        var attacker = team[new Random().Next(team.Count)].Pokemon;
        var opponent = await GetPokemon((new Random().Next(151) + 1).ToString());

        var move = attacker.Moves[new Random().Next(attacker.Moves.Count)];
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
    /// Constructs a <see cref="PokeQuizModels.Matchup"/>.
    /// </summary>
    /// <returns>The matchup containing two <see cref="PokeQuizModels.Pokemon"/> and a <see cref="PokeQuizModels.Move"/></returns>
    public async Task<PokeQuizModels.Matchup> PostMatchup(PokeQuizModels.Matchup matchup)
    {
        if (matchup.Guess is null || matchup.Guess != matchup.Effectiveness)
        {
            matchup.Team.Find(member => member.Pokemon.Id == matchup.Attacker.Id)!.Fainted = true;
        }

        var healthyMembers = matchup.Team.FindAll(member => !member.IsFainted);
        matchup.Attacker = healthyMembers[new Random().Next(healthyMembers.Count)].Pokemon;
        var opponent = await GetPokemon((new Random().Next(151) + 1).ToString());

        matchup.Move = matchup.Attacker.Moves[new Random().Next(matchup.Attacker.Moves.Count)];

        matchup.Effectiveness = typeService.CalculateEffectiveness(matchup.Move.Type, opponent.Types);
        matchup.Guess = null;

        return matchup;
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

        foreach (var value in Enum.GetValues<PokeQuizModels.Types>())
        {
            response.Add(new PokeAPIModels.NamedApiResource<PokeAPIModels.Type>
            {
                Name = value.ToString().ToLowerInvariant(),
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
    /// Get a team of <see cref="PokeQuizModels.TeamMember">Pokemon</see>.
    /// </summary>
    /// <param name="size">The size of the team</param>
    /// <returns>A list of <see cref="PokeQuizModels.Pokemon">Pokemon</see></returns>
    private async Task<List<PokeQuizModels.TeamMember>> GetTeam(int size = 6)
    {
        var tasks = new List<Task<PokeQuizModels.TeamMember>>();
        for (var i = 0; i < size; i++)
        {
            tasks.Add(GetTeamMember());
        }

        return (await Task.WhenAll(tasks)).ToList();
    }

    /// <summary>
    /// Get a <see cref="PokeQuizModels.TeamMember">Team Member</see>.
    /// </summary>
    /// <returns>A <see cref="PokeQuizModels.TeamMember">TeamMember</see> containing a <see cref="PokeQuizModels.Pokemon"/> and its state</returns>
    private async Task<PokeQuizModels.TeamMember> GetTeamMember()
    {
        var pokemon = await GetPokemon((new Random().Next(151) + 1).ToString());
        pokemon.Moves = pokemon.Moves.FindAll(move => move.IsAttackingMove).Take(4).ToList();

        return new PokeQuizModels.TeamMember
        {
            Pokemon = pokemon,
            Fainted = false
        };
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