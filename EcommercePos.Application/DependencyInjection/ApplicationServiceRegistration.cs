using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EcommercePos.Application.DependencyInjection;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Auto-register all nested Handler classes as scoped services
        var handlerTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name == "Handler" && t.IsNested)
            .ToList();

        foreach (var handlerType in handlerTypes)
        {
            services.AddScoped(handlerType);
        }

        // Auto-register all FluentValidation validators
        services.AddValidatorsFromAssembly(assembly, ServiceLifetime.Scoped);

        return services;
    }
}
