using PokeQuiz.Services;
using PokeQuizModels = PokeQuiz.Models.PokeQuiz;

namespace PokeQuiz.Endpoints.Move;

public class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/move/{id}", async (string id, IPokeQuizService service) => await service.GetMove(id))
            .WithName("GetMove")
            .WithTags("Move")
            .WithOpenApi()
            .Produces<PokeQuizModels.Move>();
    }
}