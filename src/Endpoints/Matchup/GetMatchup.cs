using PokeQuiz.Services;
using PokeQuizModels = PokeQuiz.Models.PokeQuiz;

namespace PokeQuiz.Endpoints.Matchup;

public class GetMatchup : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/matchup", async (IPokeQuizService service) => await service.GetMatchup())
            .WithName("GetMatchup")
            .WithTags("Matchup")
            .WithOpenApi()
            .Produces(404)
            .Produces(500)
            .Produces<PokeQuizModels.Matchup>();
    }
}