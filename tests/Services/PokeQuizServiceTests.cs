using PokeQuiz.Services;
using RichardSzalay.MockHttp;
using PokeQuizModels = PokeQuiz.Models.PokeQuiz;

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
            var response = File.ReadAllText(Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/move/pound.json"));
            mockHttp.When("https://pokeapi.co/api/v2/move/*").Respond("application/json", response);
        }
        {
            var response = File.ReadAllText(Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/type/normal.json"));
            mockHttp.When("https://pokeapi.co/api/v2/type/*").Respond("application/json", response);
        }

        _pokeQuizService = new PokeQuizService(mockHttp.ToHttpClient(), _typeEffectivenessService);
    }

    [Fact]
    public async Task PokeQuizService_GetsAPokemonById()
    {
        var pokemon = await _pokeQuizService.GetPokemon("bulbasaur");

        Assert.IsType<PokeQuizModels.Pokemon>(pokemon);
        Assert.Equal(1, pokemon.Id);
        Assert.Equal("bulbasaur", pokemon.Name);
    }

    [Fact]
    public async Task PokeQuizService_GetsASpeciesById()
    {
        var species = await _pokeQuizService.GetSpecies("bulbasaur");

        Assert.IsType<PokeQuizModels.PokemonSpecies>(species);
        Assert.Equal(1, species.Id);
        Assert.Equal("bulbasaur", species.Name);
    }

    [Fact]
    public async Task PokeQuizService_GetsAMoveById()
    {
        var move = await _pokeQuizService.GetMove("pound");

        Assert.IsType<PokeQuizModels.Move>(move);
        Assert.Equal(1, move.Id);
        Assert.Equal("pound", move.Name);
    }

    [Fact]
    public async Task PokeQuizService_GetsATypeById()
    {
        var type = await _pokeQuizService.GetType("normal");

        Assert.IsType<PokeQuizModels.Type>(type);
        Assert.Equal(1, type.Id);
        Assert.Equal("normal", type.Name);
    }

    [Fact]
    public async Task PokeQuizService_GetsAnOverviewOfAllTypes()
    {
        var typesResponse = await _pokeQuizService.GetTypes();
        var pokemonTypes = typeof(PokeQuizModels.Types).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

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

        Assert.NotNull(matchup.Attacker);
        Assert.IsType<PokeQuizModels.Pokemon>(matchup.Attacker);

        Assert.NotNull(matchup.Defender);
        Assert.IsType<PokeQuizModels.Pokemon>(matchup.Defender);

        Assert.NotNull(matchup.Move);
        Assert.IsType<PokeQuizModels.Move>(matchup.Move);
        Assert.True(matchup.Move.Power > 0);

        Assert.Equal(PokeQuizModels.TypeEffectiveness.Effective, matchup.Effectiveness);
    }
}