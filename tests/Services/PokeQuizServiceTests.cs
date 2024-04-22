using PokeQuiz.Models.PokeQuiz;
using PokeQuiz.Services;
using RichardSzalay.MockHttp;

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
            mockHttp.When("https://pokeapi.co/api/v2/pokemon/*/").Respond("application/json", response);
        }
        {
            var response = File.ReadAllText(Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/pokemon-species/bulbasaur.json"));
            mockHttp.When("https://pokeapi.co/api/v2/pokemon-species/*/").Respond("application/json", response);
        }
        {
            var response = File.ReadAllText(Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/move/pound.json"));
            mockHttp.When("https://pokeapi.co/api/v2/move/*/").Respond("application/json", response);
        }
        {
            var response = File.ReadAllText(Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/type/normal.json"));
            mockHttp.When("https://pokeapi.co/api/v2/type/*/").Respond("application/json", response);
        }

        _pokeQuizService = new PokeQuizService(mockHttp.ToHttpClient(), _typeEffectivenessService);
    }

    [Fact]
    public async Task PokeQuizService_GetsAPokemonById()
    {
        var pokemon = await _pokeQuizService.GetPokemon(1);

        Assert.IsType<Pokemon>(pokemon);
    }

    [Fact]
    public async Task PokeQuizService_GetsAMoveById()
    {
        var move = await _pokeQuizService.GetMove(1);

        Assert.IsType<Move>(move);
    }

    [Fact]
    public async Task PokeQuizService_GetsATypeById()
    {
        var type = await _pokeQuizService.GetType(1);

        Assert.IsType<PokeQuiz.Models.PokeQuiz.Type>(type);
    }

    [Fact]
    public async Task PokeQuizService_GetsAMatchup()
    {
        var matchup = await _pokeQuizService.GetMatchup();

        Assert.IsType<Matchup>(matchup);

        Assert.NotNull(matchup.Attacker);
        Assert.IsType<Pokemon>(matchup.Attacker);

        Assert.NotNull(matchup.Defender);
        Assert.IsType<Pokemon>(matchup.Defender);

        Assert.NotNull(matchup.Move);
        Assert.IsType<Move>(matchup.Move);
        Assert.True(matchup.Move.Power > 0);

        Assert.Equal(TypeEffectiveness.Effective, matchup.Effectiveness);
    }
}