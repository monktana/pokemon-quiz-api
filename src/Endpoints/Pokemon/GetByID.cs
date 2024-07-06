using PokeQuiz.Services;
using PokeQuizModels = PokeQuiz.Models.PokeQuiz;

namespace PokeQuiz.Endpoints.Pokemon;

public class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/pokemon/{id}", async (string id, IPokeQuizService service) => await service.GetPokemon(id))
            .WithName("GetPokemon")
            .WithTags("Pokemon")
            .WithOpenApi()
            .Produces<PokeQuizModels.Pokemon>();
    }
}