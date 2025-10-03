namespace Sigapi.Common.Filters;

internal sealed class ValidationFilter<TRequest> : IEndpointFilter where TRequest : class
{
    private readonly IValidator<TRequest> validator;

    public ValidationFilter(IValidator<TRequest> validator)
    {
        this.validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<TRequest>().First();
        var result = await validator.ValidateAsync(request, context.HttpContext.RequestAborted);

        return result.IsValid 
            ? await next(context) 
            : TypedResults.ValidationProblem(result.ToDictionary());
    }
}