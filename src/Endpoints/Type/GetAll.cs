using PokeQuiz.Services;
using PokeAPIModels = PokeQuiz.Models.PokeApi;

namespace PokeQuiz.Endpoints.Type;

public class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/types", async (IPokeQuizService service, HttpContext context) =>
            {
                var version = context.GetRequestedApiVersion();

                var types = await service.GetTypes();
                var result = types.Select(type =>
                {
                    PokeAPIModels.UrlNavigation<PokeAPIModels.Type> value = type;
                    value.Url = new Uri($"{context.Request.Scheme}://{context.Request.Host}/api/v{version}/type/{type.Name}").ToString();

                    return value;
                });

                return result;
            })
            .WithName("GetTypes")
            .WithTags("PokemonType")
            .WithOpenApi()
            .Produces<List<PokeAPIModels.NamedApiResource<PokeAPIModels.Type>>>();
    }
}