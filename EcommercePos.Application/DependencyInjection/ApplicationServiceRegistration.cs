using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EcommercePos.Application.DependencyInjection;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var handlerTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name == "Handler" && t.IsNested)
            .ToList();

        foreach (var handlerType in handlerTypes)
        {
            services.AddScoped(handlerType);
        }

        services.AddValidatorsFromAssembly(assembly, ServiceLifetime.Scoped);

        return services;
    }
}
