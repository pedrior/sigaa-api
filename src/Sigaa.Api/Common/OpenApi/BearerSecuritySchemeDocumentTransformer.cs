using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Sigaa.Api.Common.OpenApi;

internal sealed class BearerSecuritySchemeDocumentTransformer : IOpenApiDocumentTransformer
{
    private readonly IAuthenticationSchemeProvider authenticationSchemeProvider;

    public BearerSecuritySchemeDocumentTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider)
    {
        this.authenticationSchemeProvider = authenticationSchemeProvider;
    }

    public async Task TransformAsync(OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.All(authScheme => authScheme.Name != JwtBearerDefaults.AuthenticationScheme))
        {
            return;
        }

        // Add the security scheme at the document level.
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            [JwtBearerDefaults.AuthenticationScheme] = new OpenApiSecurityScheme()
            {
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "Json Web Token"
            }
        };

        // Create the security requirement which all non-public endpoints will use.
        var securityRequirement = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme),
                [] // The list of scopes is empty for Bearer JWT.
            }
        };

        // Add the security requirement to each non-public endpoint.
        foreach (var group in context.DescriptionGroups)
        {
            foreach (var description in group.Items)
            {
                if (IsPublicEndpoint(description))
                {
                    continue;
                }

                if (!document.Paths.TryGetValue($"/{description.RelativePath}", out var pathItem))
                {
                    continue;
                }

                var method = new HttpMethod(description.HttpMethod!);
                if (pathItem.Operations is not { } operations || 
                    operations.GetValueOrDefault(method) is not { } operation)
                {
                    continue;
                }

                operation.Security ??= new List<OpenApiSecurityRequirement>();
                operation.Security.Add(securityRequirement);
            }
        }
    }

    private static bool IsPublicEndpoint(ApiDescription description)
    {
        var metadata = description.ActionDescriptor.EndpointMetadata;

        var hasAuthorize = metadata.Any(m => m is AuthorizeAttribute);
        var hasAllowAnonymous = metadata.Any(m => m is AllowAnonymousAttribute);

        return !hasAuthorize || hasAllowAnonymous;
    }
}