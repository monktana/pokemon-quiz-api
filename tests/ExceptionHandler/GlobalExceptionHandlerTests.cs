using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Newtonsoft.Json;
using PokeQuiz.Services;
using RichardSzalay.MockHttp;

namespace PokeQuiz.UnitTests.ExceptionHandler;

public class GlobalExceptionHandlerTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly HttpRequestException NotFoundException = new("Not Found", null, HttpStatusCode.NotFound);
    private static readonly ArgumentException ArgumentException = new("Invalid Argument Provided");

    private readonly Mock<IPokeQuizService> _mockPokeQuizService = new();
    private readonly TypeEffectivenessService _typeEffectivenessService = new(Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/type", "PokemonTypeMatrix.json"));

    [Fact]
    public async Task TryHandleAsync_HandlesHttpRequestException()
    {
        _mockPokeQuizService.Setup(service => service.GetPokemon("snasen")).Throws(NotFoundException);

        var client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IPokeQuizService>();
                services.AddSingleton<IPokeQuizService>(_ => _mockPokeQuizService.Object);
            });
        }).CreateClient();

        var response = await client.GetAsync($"api/v1/pokemon/snasen");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var actual = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync())!;
        Assert.Equal((int)HttpStatusCode.NotFound, (int)actual.status);
        Assert.Equal("Not Found", (string)actual.title);
    }

    [Fact]
    public async Task TryHandleAsync_HandlesNonHttpRequestException()
    {
        _mockPokeQuizService.Setup(service => service.GetMatchup()).Throws(ArgumentException);

        var client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IPokeQuizService>();
                services.AddSingleton<IPokeQuizService>(_ => _mockPokeQuizService.Object);
            });
        }).CreateClient();

        var response = await client.GetAsync($"api/v1/matchup");
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        var actual = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync())!;
        Assert.Equal((int)HttpStatusCode.InternalServerError, (int)actual.status);
        Assert.Equal("Internal Server Error", (string)actual.title);
    }

    [Fact]
    public async Task TryHandleAsync_IndirectHttpException_GetCorrectStatusCode()
    {
        var mockHttp = new MockHttpMessageHandler();
        {
            var notFound = new HttpResponseMessage(HttpStatusCode.NotFound);
            mockHttp.When("https://pokeapi.co/api/v2/pokemon/*").Respond(_ => notFound);
        }

        var client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IPokeQuizService>();
                services.AddSingleton<IPokeQuizService>(new PokeQuizService(mockHttp.ToHttpClient(), _typeEffectivenessService));
            });
        }).CreateClient();

        var response = await client.GetAsync($"api/v1/pokemon/snasen");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}