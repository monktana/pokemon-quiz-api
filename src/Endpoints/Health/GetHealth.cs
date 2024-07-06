namespace PokeQuiz.Endpoints.Health;

public class GetHealth : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapHealthChecks("/health")
            .WithName("GetHealth")
            .WithTags("Health")
            .WithOpenApi();
    }
}