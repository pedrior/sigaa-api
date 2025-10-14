using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.RateLimiting;

namespace Sigaa.Api.Common.RateLimiting;

internal static class DependencyInjectionExtensions
{
    public static WebApplicationBuilder AddRateLimitingServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRateLimiter(options =>
        {
            options.ConfigureRateLimitResponse();

            options.ConfigureGlobalRateLimiterPolicy();
            options.ConfigureSessionManagementRateLimiterPolicy();
            options.ConfigureAuthenticatedRateLimiterPolicy();
        });

        return builder;
    }

    private static void ConfigureRateLimitResponse(this RateLimiterOptions options)
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        options.OnRejected = async (rejectedContext, cancellationToken) =>
        {
            if (rejectedContext.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            {
                rejectedContext.HttpContext.Response.Headers.RetryAfter = $"{retryAfter.TotalSeconds}";
            }

            var problemDetailsFactory = rejectedContext.HttpContext.RequestServices
                .GetRequiredService<ProblemDetailsFactory>();

            var problemDetails = problemDetailsFactory.CreateProblemDetails(
                rejectedContext.HttpContext,
                statusCode: StatusCodes.Status429TooManyRequests);

            await rejectedContext.HttpContext.Response.WriteAsJsonAsync(
                problemDetails,
                cancellationToken: cancellationToken);
        };
    }

    private static void ConfigureGlobalRateLimiterPolicy(this RateLimiterOptions options)
    {
        // Limit requests to 100 per minute per client IP.
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            RateLimitPartition.GetFixedWindowLimiter(
                GetClientIp(context),
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1)
                }));
    }

    private static void ConfigureSessionManagementRateLimiterPolicy(this RateLimiterOptions options)
    {
        // Limit to 5 requests per minute per client IP.
        options.AddPolicy(
            RateLimiterPolicies.Account.SessionManagement,
            context => RateLimitPartition.GetTokenBucketLimiter(
                GetClientIp(context),
                _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 5,
                    TokensPerPeriod = 5,
                    ReplenishmentPeriod = TimeSpan.FromMinutes(1)
                }));
    }

    private static void ConfigureAuthenticatedRateLimiterPolicy(this RateLimiterOptions options)
    {
        // Limit to 25 requests per minute per client IP.
        options.AddPolicy(
            RateLimiterPolicies.Authenticated,
            context => RateLimitPartition.GetTokenBucketLimiter(
                GetClientIp(context),
                _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 25,
                    TokensPerPeriod = 25,
                    ReplenishmentPeriod = TimeSpan.FromMinutes(1)
                }));
    }

    private static string GetClientIp(HttpContext context) =>
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
}