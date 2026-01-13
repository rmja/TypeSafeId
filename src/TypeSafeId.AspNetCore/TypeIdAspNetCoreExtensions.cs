using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using TypeSafeId.AspNetCore;

namespace TypeSafeId;

/// <summary>
/// Provides extension methods for configuring TypeId route constraint support in ASP.NET Core routing.
/// </summary>
public static class TypeIdAspNetCoreExtensions
{
    /// <summary>
    /// Adds TypeId route constraint support.
    /// </summary>
    public static IServiceCollection AddTypeIdRouting(this IServiceCollection services)
    {
        services.Configure<RouteOptions>(options =>
        {
            options.SetParameterPolicy<TypeIdRouteConstraint>("typeid");
        });

        return services;
    }
}
