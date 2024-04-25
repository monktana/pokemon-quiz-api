using System.Reflection;
using PokeQuiz.Endpoints;
using PokeQuiz.Middleware;
using PokeQuiz.Models;
using PokeQuiz.Services;
using PokeQuiz.Services.MessageHandler;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions<CorsSettings>()
    .BindConfiguration(CorsSettings.Position)
    .ValidateDataAnnotations()
    .ValidateOnStart();

var corsOptions = builder.Configuration.GetSection(CorsSettings.Position).Get<CorsSettings>();
builder.Services.Configure<CorsSettings>(builder.Configuration.GetSection(CorsSettings.Position));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecific",
        policy =>
        {
            policy.WithOrigins(corsOptions.AllowedOrigins)
                .AllowAnyHeader()
                .WithMethods(corsOptions.AllowedMethods);
        });
});

builder.Services.AddTransient<HttpExceptionHandlerMiddleware>();
builder.Services.AddProblemDetails();

builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = builder.Configuration.GetConnectionString("Redis"); });
builder.Services.AddSingleton<TypeEffectivenessService>(_ => new TypeEffectivenessService(Path.Join(Directory.GetCurrentDirectory(), "Data", "PokemonTypeMatrix.json")));
builder.Services.AddSingleton<RedisCache>();
builder.Services.AddHttpClient<IPokeQuizService, PokeQuizService>().AddHttpMessageHandler<RedisCache>();
builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.UseCors("AllowSpecific");

app.UseMiddleware<HttpExceptionHandlerMiddleware>();
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