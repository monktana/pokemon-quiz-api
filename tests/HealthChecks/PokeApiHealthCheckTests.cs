using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PokeQuiz.HealthChecks;
using RichardSzalay.MockHttp;

namespace PokeQuiz.UnitTests.HealthChecks;

public class PokeApiHealthCheckTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task CheckHealthAsync_ReturnsHealthy_WhenResponseCodeIsOk()
    {
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("https://pokeapi.co/api/v2*").Respond(HttpStatusCode.OK);

        var client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.Configure<HealthCheckServiceOptions>(options => { options.Registrations.Clear(); });
                services.AddHealthChecks().AddTypeActivatedCheck<PokeApiHealthCheck>(name: "PokeApi", failureStatus: HealthStatus.Unhealthy, tags: Array.Empty<string>(), args: new object[] { mockHttp.ToHttpClient() });
            });
        }).CreateClient();

        var response = await client.GetAsync($"api/v1/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task CheckHealthAsync_ReturnsUnhealthy_WhenResponseCodeIsNotOk()
    {
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("https://pokeapi.co/api/v2*").Respond(HttpStatusCode.ServiceUnavailable);

        var client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.Configure<HealthCheckServiceOptions>(options => { options.Registrations.Clear(); });
                services.AddHealthChecks().AddTypeActivatedCheck<PokeApiHealthCheck>(name: "PokeApi", failureStatus: HealthStatus.Unhealthy, tags: Array.Empty<string>(), args: new object[] { mockHttp.ToHttpClient() });
            });
        }).CreateClient();

        var response = await client.GetAsync($"api/v1/health");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Equal("Unhealthy", await response.Content.ReadAsStringAsync());
    }
}