using PokeQuiz.Models.PokeQuiz;
using PokeQuiz.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<PokeQuizService>().AddHttpMessageHandler(_ => new FileCache());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/matchup", async (PokeQuizService service) => await service.GetMatchup()).WithName("GetMatchup")
    .WithOpenApi()
    .Produces<Matchup>();

app.Run();