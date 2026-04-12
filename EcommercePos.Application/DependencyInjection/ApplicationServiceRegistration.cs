using System.Reflection;
using EcommercePos.Application.Caching;
using EcommercePos.Application.Events;
using EcommercePos.Application.Repositories;
using EcommercePos.Application.Services;
using EcommercePos.Application.UnitOfWork;
using EcommercePos.Persistence.Data;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

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

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped(typeof(IReadOnlyRepository<>), typeof(ReadOnlyRepository<>));
        services.AddScoped(typeof(IRepositoryWithInclude<>), typeof(RepositoryWithInclude<>));

        services.AddScoped<IUnitOfWork, EcommercePos.Application.UnitOfWork.UnitOfWork>();
        services.AddScoped<IUnitOfWork<ApplicationDbContext>, EcommercePos.Application.UnitOfWork.UnitOfWork<ApplicationDbContext>>();

        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddScoped<IEventDispatcher, EventDispatcher>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        services.AddScoped<ICartService, CartService>();

        return services;
    }
}
