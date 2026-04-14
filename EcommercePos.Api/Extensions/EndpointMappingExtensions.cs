using System.Reflection;

namespace EcommercePos.Api.Extensions;

public static class EndpointMappingExtensions
{
    public static IEndpointRouteBuilder MapAllEndpoints(this IEndpointRouteBuilder app)
    {
        var methods = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsAbstract && t.IsSealed && t.Namespace == "EcommercePos.Api.Endpoints")
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
            .Where(m => m.Name.StartsWith("Map") && m.Name.EndsWith("Endpoints")
                     && m.GetParameters().Length == 1
                     && m.GetParameters()[0].ParameterType == typeof(IEndpointRouteBuilder))
            .OrderBy(m => m.Name);

        foreach (var method in methods)
            method.Invoke(null, new object[] { app });

        return app;
    }
}
