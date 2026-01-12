using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace TypeSafeId.AspNetCore;

/// <summary>
/// Route constraint for TypeId values.
/// </summary>
public class TypeIdRouteConstraint : IRouteConstraint
{
    public bool Match(
        HttpContext? httpContext,
        IRouter? route,
        string routeKey,
        RouteValueDictionary values,
        RouteDirection routeDirection
    )
    {
        if (!values.TryGetValue(routeKey, out var value) || value is null)
        {
            return false;
        }

        var valueString = Convert.ToString(value, CultureInfo.InvariantCulture);
        if (string.IsNullOrEmpty(valueString))
        {
            return false;
        }

        return TypeId.TryParse(valueString, null, out _);
    }
}
