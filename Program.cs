using PokeQuiz.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient<PokeQuizService>().AddHttpMessageHandler(config => new FileCache());

var app = builder.Build();

app.MapGet("/", async (PokeQuizService service) => await service.GetMatchup());

app.Run();