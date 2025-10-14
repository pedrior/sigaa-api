using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Sigaa.Api.Common.Exceptions;

namespace Sigaa.Api;

internal static class DependencyInjectionExtensions
{
    public static WebApplicationBuilder AddApiServices(this WebApplicationBuilder builder)
    {
        builder.Services.ConfigureSerializerOptions();
        builder.Services.ConfigureProblemDetails();
        builder.Services.ConfigureForwardedHeaders();

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddSingleton<ProblemDetailsFactory, DefaultProblemDetailsFactory>();
        builder.Services.AddValidation();

        return builder;
    }

    private static void ConfigureSerializerOptions(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(
                options.SerializerOptions.PropertyNamingPolicy));
        });
    }

    private static void ConfigureProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions.Add("requestId", context.HttpContext.TraceIdentifier);
            };
        });
    }

    private static void ConfigureForwardedHeaders(this IServiceCollection services) =>
        services.Configure<ForwardedHeadersOptions>(options => options.ForwardedHeaders = ForwardedHeaders.All);
}