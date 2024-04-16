using PokeQuiz.Clients;
using PokeQuiz.Models.PokeApi;

namespace PokeQuiz.UnitTests.Clients;

public class PokeApiClientTests
{
    [Fact]
    public async Task GetResourceAsyncReturnsTheResourceById()
    {
        var client = new PokeApiClient(new HttpClient());
        var resource = await client.GetResourceAsync<Pokemon>(1);

        Assert.IsType<Pokemon>(resource);
        Assert.Equal(1, resource.Id);
        Assert.Equal("bulbasaur", resource.Name);
    }

    [Fact]
    public async Task GetResourceAsyncReturnsTheResourceByName()
    {
        var client = new PokeApiClient(new HttpClient());
        var resource = await client.GetResourceAsync<Pokemon>("bulbasaur");

        Assert.IsType<Pokemon>(resource);
        Assert.Equal(1, resource.Id);
        Assert.Equal("bulbasaur", resource.Name);
    }

    [Fact]
    public async Task GetResourceAsyncReturnsAListOfResources()
    {
        var client = new PokeApiClient(new HttpClient());
        var urlList = new List<NamedApiResource<Pokemon>>
        {
            new() { Url = "http://pokeapi.co/api/v2/pokemon/1", Name = "bulbasaur" },
            new() { Url = "http://pokeapi.co/api/v2/pokemon/4", Name = "charmander" }
        };

        var resources = await client.GetResourceAsync(urlList);

        Assert.IsType<List<Pokemon>>(resources);

        for (var i = 0; i < resources.Count; i++)
        {
            Assert.IsType<Pokemon>(resources[i]);
            Assert.Equal(urlList[i].Name, resources[i].Name);
        }
    }
}