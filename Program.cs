using PokeQuiz.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient<PokeApiService>().AddHttpMessageHandler(config => new FileCache());

var app = builder.Build();

app.MapGet("/", async (PokeApiService service) => await service.GetMatchup());

app.Run();
