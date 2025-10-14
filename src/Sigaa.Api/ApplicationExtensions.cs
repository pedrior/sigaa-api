using Microsoft.AspNetCore.Authentication.JwtBearer;
using Scalar.AspNetCore;
using Serilog;

namespace Sigaa.Api;

internal static class ApplicationExtensions
{
    public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration));

        return builder;
    }

    public static WebApplication UseScalarUi(this WebApplication app)
    {
        app.MapOpenApi();

        app.MapScalarApiReference("/docs", options =>
        {
            options.AddPreferredSecuritySchemes(JwtBearerDefaults.AuthenticationScheme);
            options.WithTitle("SIGAA UFPB API")
                .WithTheme(ScalarTheme.Default)
                .WithDynamicBaseServerUrl()
                .WithSchemaPropertyOrder(PropertyOrder.Preserve)
                .WithDefaultHttpClient(ScalarTarget.Node, ScalarClient.Axios);
        });

        // Redirect to docs.
        app.MapGet("/", () => Results.Redirect("/docs", permanent: true))
            .ExcludeFromApiReference();

        return app;
    }
}