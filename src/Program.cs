using PokeQuiz.Middleware;
using PokeQuiz.Services;
using PokeQuizModels = PokeQuiz.Models.PokeQuiz;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecific",
        policy =>
        {
            policy.WithOrigins(builder.Configuration.GetValue<string>("AllowedOrigins"))
                .AllowAnyHeader().AllowAnyMethod();
        });
});
builder.Services.AddProblemDetails();

builder.Services.AddSingleton<TypeEffectivenessService>(_ => new TypeEffectivenessService(Path.Join(Directory.GetCurrentDirectory(), "Data", "PokemonTypeMatrix.json")));
builder.Services.AddHttpClient<IPokeQuizService, PokeQuizService>().AddHttpMessageHandler(_ => new FileCache());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<HttpExceptionHandlerMiddleware>();

app.UseStatusCodePages(async statusCodeContext
    => await Results.Problem(statusCode: statusCodeContext.HttpContext.Response.StatusCode)
        .ExecuteAsync(statusCodeContext.HttpContext));

app.UseCors("AllowSpecific");

app.MapGet("/pokemon/{id}", async (string id, IPokeQuizService service) => await service.GetPokemon(id))
    .WithName("GetPokemon")
    .WithOpenApi()
    .Produces(404)
    .Produces(500)
    .Produces<PokeQuizModels.Pokemon>();

app.MapGet("/species/{id}", async (string id, IPokeQuizService service) => await service.GetSpecies(id))
    .WithName("GetSpecies")
    .WithOpenApi()
    .Produces(404)
    .Produces(500)
    .Produces<PokeQuizModels.PokemonSpecies>();

app.MapGet("/type/{id}", async (string id, IPokeQuizService service) => await service.GetType(id))
    .WithName("GetType")
    .WithOpenApi()
    .Produces(404)
    .Produces(500)
    .Produces<PokeQuizModels.Type>();

app.MapGet("/move/{id}", async (string id, IPokeQuizService service) => await service.GetMove(id))
    .WithName("GetMove")
    .WithOpenApi()
    .Produces(404)
    .Produces(500)
    .Produces<PokeQuizModels.Move>();

app.MapGet("/matchup", async (IPokeQuizService service) => await service.GetMatchup())
    .WithName("GetMatchup")
    .WithOpenApi()
    .Produces(404)
    .Produces(500)
    .Produces<PokeQuizModels.Matchup>();

app.Run();

public partial class Program
{
}