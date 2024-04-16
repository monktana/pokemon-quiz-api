using PokeQuiz.Middleware;
using PokeQuiz.Models.PokeQuiz;
using PokeQuiz.Services;
using Type = PokeQuiz.Models.PokeQuiz.Type;

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

builder.Services.AddSingleton<TypeEffectivenessService>();
builder.Services.AddHttpClient<PokeQuizService>().AddHttpMessageHandler(_ => new FileCache());

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

app.MapGet("/pokemon/{id}", async (string id, PokeQuizService service) => await service.GetPokemon(id))
    .WithName("GetPokemon")
    .WithOpenApi()
    .Produces(404)
    .Produces(500)
    .Produces<Pokemon>();

app.MapGet("/species/{id}", async (string id, PokeQuizService service) => await service.GetSpecies(id))
    .WithName("GetSpecies")
    .WithOpenApi()
    .Produces(404)
    .Produces(500)
    .Produces<PokemonSpecies>();

app.MapGet("/type/{id}", async (string id, PokeQuizService service) => await service.GetType(id))
    .WithName("GetType")
    .WithOpenApi()
    .Produces(404)
    .Produces(500)
    .Produces<Type>();

app.MapGet("/move/{id}", async (string id, PokeQuizService service) => await service.GetMove(id))
    .WithName("GetMove")
    .WithOpenApi()
    .Produces(404)
    .Produces(500)
    .Produces<Move>();

app.MapGet("/matchup", async (PokeQuizService service) => await service.GetMatchup())
    .WithName("GetMatchup")
    .WithOpenApi()
    .Produces(404)
    .Produces(500)
    .Produces<Matchup>();

app.Run();