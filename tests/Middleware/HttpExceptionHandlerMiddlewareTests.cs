using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using PokeQuiz.Models.PokeQuiz;
using PokeQuiz.Services;
using Type = PokeQuiz.Models.PokeQuiz.Type;

namespace PokeQuiz.UnitTests.Middleware;

public class HttpExceptionHandlerMiddlewareTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HttpExceptionHandlerMiddlewareTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder => { builder.ConfigureTestServices(services => { services.AddHttpClient<IPokeQuizService, NullPokeQuizService>(); }); }).CreateClient();
    }

    [Fact]
    public async Task HttpExceptionHandlerMiddleware_ReturnsNotFoundForRequest()
    {
        var response = await _client.GetAsync("/pokemon/1");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task HttpExceptionHandlerMiddleware_ReturnsBadRequestForRequest()
    {
        var response = await _client.GetAsync("/move/1");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task HttpExceptionHandlerMiddleware_RethrowsOnNonHttpRequestExceptions()
    {
        var response = await _client.GetAsync("/matchup");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}

public class NullPokeQuizService(HttpClient httpClient) : IPokeQuizService
{
    public Task<Matchup> GetMatchup()
    {
        throw new NotImplementedException();
    }

    public Task<Pokemon> GetPokemon(string name)
    {
        throw new HttpRequestException("Not Found", null, HttpStatusCode.NotFound);
    }

    public Task<PokemonSpecies> GetSpecies(string name)
    {
        throw new NotImplementedException();
    }

    public Task<Type> GetType(string name)
    {
        throw new NotImplementedException();
    }

    public Task<Move> GetMove(string name)
    {
        throw new HttpRequestException("Bad Request", null, HttpStatusCode.BadRequest);
    }
}