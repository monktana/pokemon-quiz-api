using System.Net;
using System.Reflection;
using Newtonsoft.Json;
using PokeQuiz.Services;
using RichardSzalay.MockHttp;
using PokeQuizModels = PokeQuiz.Models.PokeQuiz;
using PokeApiModels = PokeQuiz.Models.PokeApi;

namespace PokeQuiz.UnitTests.Services;

public class PokeQuizServiceTests
{
    private readonly PokeQuizService _pokeQuizService;
    private readonly TypeEffectivenessService _typeEffectivenessService = new(Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/type", "PokemonTypeMatrix.json"));

    public PokeQuizServiceTests()
    {
        var mockHttp = new MockHttpMessageHandler();
        {
            var response = File.ReadAllText(Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/pokemon/bulbasaur.json"));
            mockHttp.When("https://pokeapi.co/api/v2/pokemon/*").Respond("application/json", response);
        }
        {
            var response = File.ReadAllText(Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/pokemon-species/bulbasaur.json"));
            mockHttp.When("https://pokeapi.co/api/v2/pokemon-species/*").Respond("application/json", response);
        }
        {
            var tackle = File.ReadAllText(Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/move/tackle.json"));
            var pound = File.ReadAllText(Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/move/pound.json"));

            mockHttp.When("https://pokeapi.co/api/v2/move/tackle").Respond("application/json", tackle);
            mockHttp.When("https://pokeapi.co/api/v2/move/snasen").Respond(HttpStatusCode.NotFound);
            mockHttp.When("https://pokeapi.co/api/v2/move/*").Respond("application/json", pound);
        }
        {
            var normal = File.ReadAllText(Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/type/normal.json"));
            var ghost = File.ReadAllText(Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/type/ghost.json"));

            mockHttp.When("https://pokeapi.co/api/v2/type/ghost").Respond("application/json", ghost);
            mockHttp.When("https://pokeapi.co/api/v2/type/snasen").Respond(HttpStatusCode.NotFound);
            mockHttp.When("https://pokeapi.co/api/v2/type/*").Respond("application/json", normal);
        }

        _pokeQuizService = new PokeQuizService(mockHttp.ToHttpClient(), _typeEffectivenessService);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("bulbasaur")]
    public async Task PokeQuizService_GetsAPokemonByIdentifier(string identifier)
    {
        var pokemon = await _pokeQuizService.GetPokemon(identifier);

        Assert.IsType<PokeQuizModels.Pokemon>(pokemon);
        Assert.Equal(1, pokemon.Id);
        Assert.Equal("bulbasaur", pokemon.Name);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("bulbasaur")]
    public async Task PokeQuizService_GetsASpeciesByIdentifier(string identifier)
    {
        var species = await _pokeQuizService.GetSpecies(identifier);

        Assert.IsType<PokeQuizModels.PokemonSpecies>(species);
        Assert.Equal(1, species.Id);
        Assert.Equal("bulbasaur", species.Name);
    }

    [Fact]
    public async Task PokeQuizService_GetsASpeciesByUrl()
    {
        var species = await _pokeQuizService.GetSpecies(new PokeApiModels.NamedApiResource<PokeApiModels.PokemonSpecies>
        {
            Url = "https://pokeapi.co/api/v2/pokemon-species/1",
            Name = "bulbasaur"
        });

        Assert.IsType<PokeQuizModels.PokemonSpecies>(species);
        Assert.Equal(1, species.Id);
        Assert.Equal("bulbasaur", species.Name);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("pound")]
    public async Task PokeQuizService_GetsAMoveByIdentifier(string identifier)
    {
        var move = await _pokeQuizService.GetMove(identifier);

        Assert.IsType<PokeQuizModels.Move>(move);
        Assert.Equal(1, move.Id);
        Assert.Equal("pound", move.Name);
    }

    [Fact]
    public async Task PokeQuizService_GetsAMoveByUrl()
    {
        var move = await _pokeQuizService.GetMove(new PokeApiModels.NamedApiResource<PokeApiModels.Move>
        {
            Url = "https://pokeapi.co/api/v2/move/1",
            Name = "pound"
        });

        Assert.IsType<PokeQuizModels.Move>(move);
        Assert.Equal(1, move.Id);
        Assert.Equal("pound", move.Name);
    }

    [Theory]
    [InlineData("pound", "tackle")]
    public async Task PokeQuizService_GetsMultipleMovesByName(params string[] names)
    {
        var moves = await _pokeQuizService.GetMoves(names.ToList());

        Assert.IsType<PokeQuizModels.Move>(moves[0]);
        Assert.Equal(1, moves[0].Id);
        Assert.Equal("pound", moves[0].Name);

        Assert.IsType<PokeQuizModels.Move>(moves[1]);
        Assert.Equal(33, moves[1].Id);
        Assert.Equal("tackle", moves[1].Name);
    }

    [Fact]
    public async Task PokeQuizService_GetMoves_ThrowIfOneRequestFails()
    {
        var exceptions = await Assert.ThrowsAsync<AggregateException>(async () => await _pokeQuizService.GetMoves(new List<string>
        {
            "pound",
            "snasen",
        }));

        Assert.Contains(exceptions.InnerExceptions, exception => exception is HttpRequestException);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("normal")]
    public async Task PokeQuizService_GetsATypeById(string identifier)
    {
        var type = await _pokeQuizService.GetType(identifier);

        Assert.IsType<PokeQuizModels.Type>(type);
        Assert.Equal(1, type.Id);
        Assert.Equal("normal", type.Name);
    }

    [Fact]
    public async Task PokeQuizService_GetsATypeByUrl()
    {
        var type = await _pokeQuizService.GetType(new PokeApiModels.NamedApiResource<PokeApiModels.Type>
        {
            Url = "https://pokeapi.co/api/v2/type/1",
            Name = "normal"
        });

        Assert.IsType<PokeQuizModels.Type>(type);
        Assert.Equal(1, type.Id);
        Assert.Equal("normal", type.Name);
    }

    [Fact]
    public async Task PokeQuizService_GetsMultipleTypesByName()
    {
        var types = await _pokeQuizService.GetTypes(new List<string>
        {
            "normal",
            "ghost",
        });

        Assert.IsType<PokeQuizModels.Type>(types[0]);
        Assert.Equal(1, types[0].Id);
        Assert.Equal("normal", types[0].Name);

        Assert.IsType<PokeQuizModels.Type>(types[1]);
        Assert.Equal(8, types[1].Id);
        Assert.Equal("ghost", types[1].Name);
    }

    [Fact]
    public async Task PokeQuizService_GetTypes_ThrowIfOneRequestFails()
    {
        await Assert.ThrowsAsync<HttpRequestException>(async () => await _pokeQuizService.GetTypes(new List<string>
        {
            "normal",
            "snasen",
        }));
    }

    [Fact]
    public async Task PokeQuizService_GetsAnOverviewOfAllTypes()
    {
        var typesResponse = await _pokeQuizService.GetTypes();
        var pokemonTypes = typeof(PokeQuizModels.Types).GetFields(BindingFlags.Static | BindingFlags.Public);

        Assert.Equal(pokemonTypes.Length, typesResponse.Count);

        foreach (var pokemonType in pokemonTypes)
        {
            Assert.True(typesResponse.Exists(type => type.Name == pokemonType.Name.ToLower()));
        }
    }

    [Fact]
    public async Task PokeQuizService_GetsAMatchup()
    {
        var matchup = await _pokeQuizService.GetMatchup();

        Assert.IsType<PokeQuizModels.Matchup>(matchup);

        Assert.NotNull(matchup.Team);
        Assert.True(matchup.Team.Count > 0);
        Assert.IsType<List<PokeQuizModels.TeamMember>>(matchup.Team);

        foreach (var teamMember in matchup.Team)
        {
            Assert.NotNull(teamMember.Pokemon);
            Assert.False(teamMember.Fainted);
            Assert.IsType<PokeQuizModels.Pokemon>(teamMember.Pokemon);
            Assert.Equal(4, teamMember.Pokemon.Moves.Count);
        }

        Assert.NotNull(matchup.Attacker);
        Assert.IsType<PokeQuizModels.Pokemon>(matchup.Attacker);
        Assert.Contains(matchup.Attacker.Id, matchup.Team.Select(teamMember => teamMember.Pokemon.Id));

        Assert.NotNull(matchup.Opponent);
        Assert.IsType<PokeQuizModels.Pokemon>(matchup.Opponent);

        Assert.NotNull(matchup.Move);
        Assert.IsType<PokeQuizModels.Move>(matchup.Move);
        Assert.True(matchup.Move.Power > 0);

        Assert.Null(matchup.Guess);
    }

    [Fact]
    public async Task PokeQuizService_MarksAttackingPokemonAsFaintedWhenGuessIsIncorrect()
    {
        var matchup = await File.ReadAllTextAsync(Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/matchup/matchup.json"));
        var matchupObject = JsonConvert.DeserializeObject<PokeQuizModels.Matchup>(matchup);

        matchupObject!.Guess = PokeQuizModels.TypeEffectiveness.NotVeryEffective;

        var updatedMatchup = await _pokeQuizService.PostMatchup(matchupObject);

        Assert.True(updatedMatchup.Team.First(teamMember => teamMember.Pokemon.Id == matchupObject.Attacker.Id).Fainted);
    }

    [Fact]
    public async Task PokeQuizService_MarksAttackingPokemonAsFaintedWhenGuessIsInvalid()
    {
        var matchup = await File.ReadAllTextAsync(Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/matchup/matchup.json"));
        var matchupObject = JsonConvert.DeserializeObject<PokeQuizModels.Matchup>(matchup);

        matchupObject!.Guess = null;

        var updatedMatchup = await _pokeQuizService.PostMatchup(matchupObject);

        Assert.True(updatedMatchup.Team.First(teamMember => teamMember.Pokemon.Id == matchupObject.Attacker.Id).Fainted);
    }
}