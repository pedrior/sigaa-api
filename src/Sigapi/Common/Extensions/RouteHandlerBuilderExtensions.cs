using Sigapi.Common.Filters;

namespace Sigapi.Common.Extensions;

internal static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
        where TRequest : class
    {
        builder.AddEndpointFilter<ValidationFilter<TRequest>>()
            .ProducesValidationProblem();

        return builder;
    }
}