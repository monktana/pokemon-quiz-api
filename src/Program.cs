using System.Collections;
using System.Reflection;
using System.Text.Json.Serialization;
using Asp.Versioning;
using PokeQuiz.Endpoints;
using PokeQuiz.ExceptionHandler;
using PokeQuiz.HealthChecks;
using PokeQuiz.OpenApi;
using PokeQuiz.Services;
using PokeQuiz.Services.MessageHandler;

// console.log
foreach (DictionaryEntry environmentVariable in Environment.GetEnvironmentVariables())
{
    Console.WriteLine($"{environmentVariable.Key}={environmentVariable.Value}");
}


var builder = WebApplication.CreateBuilder(args);

Console.WriteLine($"Redis: {builder.Configuration.GetConnectionString("Redis")}");

builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureHttpJsonOptions(options => { options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
builder.Services.AddSwaggerGen();

builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddUserSecrets<Program>();

if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development")
{
    builder.WebHost.UseSentry();
}

builder.Services.AddCors(options => { options.AddPolicy("AllowAll", policy => { policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); }); });

builder.Services.AddHealthChecks().AddRedis(builder.Configuration.GetConnectionString("Redis") ?? string.Empty).AddCheck<PokeApiHealthCheck>("PokeApi");

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});
builder.Services.ConfigureOptions<ConfigureSwaggerGenOptions>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<TypeEffectivenessService>(_ => new TypeEffectivenessService(Path.Join(Directory.GetCurrentDirectory(), "Data", "PokemonTypeMatrix.json")));
builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = builder.Configuration.GetConnectionString("Redis"); });
builder.Services.AddScoped<RedisCache>();
builder.Services.AddHttpClient<IPokeQuizService, PokeQuizService>().AddHttpMessageHandler<RedisCache>();
builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.UseExceptionHandler();

app.UseStatusCodePages(async statusCodeContext
    => await Results.Problem(statusCode: statusCodeContext.HttpContext.Response.StatusCode)
        .ExecuteAsync(statusCodeContext.HttpContext));

app.UseCors("AllowAll");

var apiVersion = app.NewApiVersionSet().HasApiVersion(new ApiVersion(1)).ReportApiVersions().Build();
var apiVersionGroup = app.MapGroup("api/v{apiVersion:apiVersion}").WithApiVersionSet(apiVersion);

app.MapEndpoints(apiVersionGroup);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();
        foreach (var description in descriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }
    });
}

app.Run();

public abstract partial class Program
{
}