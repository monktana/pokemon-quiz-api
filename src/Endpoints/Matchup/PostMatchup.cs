using PokeQuiz.Services;
using PokeQuizModels = PokeQuiz.Models.PokeQuiz;

namespace PokeQuiz.Endpoints.Matchup;

public class PostMatchup : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/matchup", async (IPokeQuizService service, PokeQuizModels.Matchup matchup) => await service.PostMatchup(matchup))
            .WithName("PostMatchup")
            .WithTags("Matchup")
            .WithOpenApi()
            .Produces<PokeQuizModels.Matchup>();
    }
}