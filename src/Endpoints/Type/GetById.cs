using PokeQuiz.Services;
using PokeQuizModels = PokeQuiz.Models.PokeQuiz;

namespace PokeQuiz.Endpoints.Type;

public class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/type/{id}", async (string id, IPokeQuizService service) => await service.GetType(id))
            .WithName("GetType")
            .WithOpenApi()
            .Produces(404)
            .Produces(500)
            .Produces<PokeQuizModels.Type>();
    }
}