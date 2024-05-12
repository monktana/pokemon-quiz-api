using System.Reflection;
using System.Text.Json.Serialization;
using Asp.Versioning;
using PokeQuiz.Endpoints;
using PokeQuiz.ExceptionHandler;
using PokeQuiz.Models;
using PokeQuiz.OpenApi;
using PokeQuiz.Services;
using PokeQuiz.Services.MessageHandler;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureHttpJsonOptions(options => { options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
builder.Services.AddSwaggerGen();

builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddUserSecrets<Program>();

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
builder.Services.AddSingleton<TypeEffectivenessService>(_ => new TypeEffectivenessService(Path.Join(Directory.GetCurrentDirectory(), "Data", "PokemonTypeMatrix.json")));
builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = builder.Configuration.GetConnectionString("Redis"); });
builder.Services.AddScoped<RedisCache>();
builder.Services.AddHttpClient<IPokeQuizService, PokeQuizService>().AddHttpMessageHandler<RedisCache>();
builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

var app = builder.Build();

// https://github.com/dotnet/aspnetcore/issues/51888
app.UseExceptionHandler(o => { });

app.UseStatusCodePages(async statusCodeContext
    => await Results.Problem(statusCode: statusCodeContext.HttpContext.Response.StatusCode)
        .ExecuteAsync(statusCodeContext.HttpContext));
app.UseCors("AllowSpecific");

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