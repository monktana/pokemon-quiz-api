using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PokeQuiz.HealthChecks;

public class PokeApiHealthCheck(HttpClient client) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            var response = await client.GetAsync("https://pokeapi.co/api/v2/", cancellationToken);
            response.EnsureSuccessStatusCode();
            return HealthCheckResult.Healthy("PokeApi is available");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy("PokeApi cannot be reached", exception);
        }
    }
}