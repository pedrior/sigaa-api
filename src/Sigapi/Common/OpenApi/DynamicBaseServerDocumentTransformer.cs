using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Sigapi.Common.OpenApi;

internal sealed class DynamicBaseServerDocumentTransformer : IOpenApiDocumentTransformer
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public DynamicBaseServerDocumentTransformer(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public Task TransformAsync(OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (httpContextAccessor.HttpContext is not { } httpContext)
        {
            return Task.CompletedTask;
        }

        var request = httpContext.Request;
        var location = new OpenApiServer
        {
            // Use the current location as the base URL.
            Url = $"{request.Scheme}://{request.Host}{request.PathBase}"
        };

        document.Servers = [location];

        return Task.CompletedTask;
    }
}