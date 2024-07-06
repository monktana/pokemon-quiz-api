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
            .Produces<List<PokeQuizModels.Type>>();
    }
}