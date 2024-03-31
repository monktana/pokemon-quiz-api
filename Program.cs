using PokeQuiz.Models.PokeQuiz;
using PokeQuiz.Services;
using Type = PokeQuiz.Models.PokeQuiz.Type;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<TypeEffectivenessService>();
builder.Services.AddHttpClient<PokeQuizService>().AddHttpMessageHandler(_ => new FileCache());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/pokemon/{id}", async (string id, PokeQuizService service) => await service.GetPokemon(id))
    .WithName("GetPokemon")
    .WithOpenApi()
    .Produces<Pokemon>();

app.MapGet("/species/{id}", async (string id, PokeQuizService service) => await service.GetSpecies(id))
    .WithName("GetSpecies")
    .WithOpenApi()
    .Produces<PokemonSpecies>();

app.MapGet("/type/{id}", async (string id, PokeQuizService service) => await service.GetType(id))
    .WithName("GetType")
    .WithOpenApi()
    .Produces<Type>();

app.MapGet("/move/{id}", async (string id, PokeQuizService service) => await service.GetMove(id))
    .WithName("GetMove")
    .WithOpenApi()
    .Produces<Move>();

app.MapGet("/matchup", async (PokeQuizService service) => await service.GetMatchup())
    .WithName("GetMatchup")
    .WithOpenApi()
    .Produces<Matchup>();

app.Run();