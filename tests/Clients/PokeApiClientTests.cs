using System.Net;
using PokeQuiz.Clients;
using PokeQuiz.Models.PokeApi;
using RichardSzalay.MockHttp;

namespace PokeQuiz.UnitTests.Clients;

public class PokeApiClientTests
{
    private readonly PokeApiClient _client;

    public PokeApiClientTests()
    {
        var mockHttp = new MockHttpMessageHandler();
        {
            var response = File.ReadAllText(Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/pokemon/bulbasaur.json"));
            mockHttp.When("https://pokeapi.co/api/v2/pokemon/1/").Respond("application/json", response);
            mockHttp.When("https://pokeapi.co/api/v2/pokemon/bulbasaur/").Respond("application/json", response);
        }

        {
            var response = File.ReadAllText(Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/pokemon/charmander.json"));
            mockHttp.When("https://pokeapi.co/api/v2/pokemon/4/").Respond("application/json", response);
            mockHttp.When("https://pokeapi.co/api/v2/pokemon/charmander/").Respond("application/json", response);
        }

        _client = new PokeApiClient(mockHttp.ToHttpClient());
    }

    [Fact]
    public void PokeApiClient_CanBeDisposed()
    {
        using var client = new PokeApiClient(new HttpClient());
    }

    [Fact]
    public async Task GetResourceAsync_ReturnsTheResourceById()
    {
        var resource = await _client.GetResourceAsync<Pokemon>(1);

        Assert.IsType<Pokemon>(resource);
        Assert.Equal(1, resource.Id);
        Assert.Equal("bulbasaur", resource.Name);
    }

    [Fact]
    public async Task GetResourceAsync_ReturnsTheResourceByName()
    {
        var resource = await _client.GetResourceAsync<Pokemon>("bulbasaur");

        Assert.IsType<Pokemon>(resource);
        Assert.Equal(1, resource.Id);
        Assert.Equal("bulbasaur", resource.Name);
    }

    [Fact]
    public async Task GetResourceAsync_ReturnsTheResourceByUrl()
    {
        var resource = await _client.GetResourceAsync(new NamedApiResource<Pokemon> { Url = "https://pokeapi.co/api/v2/pokemon/1" });

        Assert.IsType<Pokemon>(resource);
        Assert.Equal(1, resource.Id);
        Assert.Equal("bulbasaur", resource.Name);
    }

    [Fact]
    public async Task GetResourceAsyncByUrl_ThrowsOnInvalidUrlFormat()
    {
        await Assert.ThrowsAsync<NotSupportedException>(async () =>
        {
            await _client.GetResourceAsync(new NamedApiResource<Pokemon>
                { Url = "https://pokeapi.co/api/v2/pokemon/snasen" });
        });
    }

    [Fact]
    public async Task GetResourceAsync_ReturnsAListOfResources()
    {
        var urlList = new List<NamedApiResource<Pokemon>>
        {
            new() { Url = "https://pokeapi.co/api/v2/pokemon/1", Name = "bulbasaur" },
            new() { Url = "https://pokeapi.co/api/v2/pokemon/4", Name = "charmander" }
        };

        var resources = await _client.GetResourceAsync(urlList);

        Assert.IsType<List<Pokemon>>(resources);

        for (var i = 0; i < resources.Count; i++)
        {
            Assert.IsType<Pokemon>(resources[i]);
            Assert.Equal(urlList[i].Name, resources[i].Name);
        }
    }

    [Fact]
    public async Task GetResourceAsync_ThrowsOnUnsuccessfulResponse()
    {
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("https://pokeapi.co/api/v2/pokemon/*").Respond(HttpStatusCode.NotFound);

        var client = new PokeApiClient(mockHttp.ToHttpClient());

        await Assert.ThrowsAsync<HttpRequestException>(async () => { await client.GetResourceAsync<Pokemon>(1); });
    }
}