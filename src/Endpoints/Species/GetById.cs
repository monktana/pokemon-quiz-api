using PokeQuiz.Services;
using PokeQuizModels = PokeQuiz.Models.PokeQuiz;

namespace PokeQuiz.Endpoints.Species;

public class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/species/{id}", async (string id, IPokeQuizService service) => await service.GetSpecies(id))
            .WithName("GetSpecies")
            .WithTags("PokemonSpecies")
            .WithOpenApi()
            .Produces(404)
            .Produces(500)
            .Produces<PokeQuizModels.PokemonSpecies>();
    }
}