using Sigapi.Common.Filters;

namespace Sigapi.Common.Extensions;

internal static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder route)
        where TRequest : class
    {
        route.AddEndpointFilter<ValidationFilter<TRequest>>()
            .ProducesValidationProblem();

        return route;
    }
}