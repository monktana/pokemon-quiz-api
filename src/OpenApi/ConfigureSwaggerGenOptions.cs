using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PokeQuiz.OpenApi;

public class ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider descriptionProvider) : IConfigureNamedOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var apiVersionDescription in descriptionProvider.ApiVersionDescriptions)
        {
            var openApiInfo = new OpenApiInfo
            {
                Title = $"PokeQuiz API v{apiVersionDescription.ApiVersion}",
                Version = apiVersionDescription.ApiVersion.ToString()
            };
            options.SwaggerDoc(apiVersionDescription.GroupName, openApiInfo);
        }
    }

    public void Configure(string? name, SwaggerGenOptions options)
    {
        Configure(options);
    }
}