using System.Reflection;
using PokeQuiz.Endpoints;
using PokeQuiz.Middleware;
using PokeQuiz.Services;

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
builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.UseMiddleware<HttpExceptionHandlerMiddleware>();
app.UseStatusCodePages(async statusCodeContext
    => await Results.Problem(statusCode: statusCodeContext.HttpContext.Response.StatusCode)
        .ExecuteAsync(statusCodeContext.HttpContext));

app.UseCors("AllowSpecific");

app.MapEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

public partial class Program
{
}