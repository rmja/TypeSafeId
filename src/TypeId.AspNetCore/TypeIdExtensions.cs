using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using TypeId.AspNetCore;

namespace TypeId;

public static class TypeIdExtensions
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
