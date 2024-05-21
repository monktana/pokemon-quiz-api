using PokeQuiz.Services;
using PokeQuizModels = PokeQuiz.Models.PokeQuiz;

namespace PokeQuiz.Endpoints.Type;

public class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/types", async (IPokeQuizService service) => await service.GetTypes())
            .WithName("GetTypes")
            .WithTags("PokemonType")
            .WithOpenApi()
            .Produces(404)
            .Produces(500)
            .Produces<List<PokeQuizModels.Type>>();
    }
}