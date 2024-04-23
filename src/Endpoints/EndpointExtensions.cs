using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace PokeQuiz.Endpoints;

public static class EndpointExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection serviceCollection, Assembly assembly)
    {
        var serviceDescriptors = assembly.DefinedTypes
            .Where(info => info is { IsAbstract: false, IsInterface: false } && info.IsAssignableTo(typeof(IEndpoint)))
            .Select(info => ServiceDescriptor.Transient(typeof(IEndpoint), info))
            .ToList();

        serviceCollection.TryAddEnumerable(serviceDescriptors);
        return serviceCollection;
    }

    public static IApplicationBuilder MapEndpoints(this WebApplication app)
    {
        var endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(app);
        }

        return app;
    }
}