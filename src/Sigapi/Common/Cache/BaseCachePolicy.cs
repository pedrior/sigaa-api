using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Primitives;

namespace Sigapi.Common.Cache;

internal sealed class BaseCachePolicy : IOutputCachePolicy
{
    ValueTask IOutputCachePolicy.CacheRequestAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        var attemptOutputCaching = AttemptOutputCaching(context);
        
        context.EnableOutputCaching = true;
        context.AllowCacheLookup = attemptOutputCaching;
        context.AllowCacheStorage = attemptOutputCaching;
        context.AllowLocking = true;

        // Vary by any query by default.
        context.CacheVaryByRules.QueryKeys = "*";

        return ValueTask.CompletedTask;
    }

    ValueTask IOutputCachePolicy.ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;

    ValueTask IOutputCachePolicy.ServeResponseAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        var response = context.HttpContext.Response;

        // Verify the existence of cookie headers.
        if (!StringValues.IsNullOrEmpty(response.Headers.SetCookie))
        {
            context.AllowCacheStorage = false;
            return ValueTask.CompletedTask;
        }

        // Check response code.
        if (response.StatusCode is not StatusCodes.Status200OK)
        {
            context.AllowCacheStorage = false;
        }

        return ValueTask.CompletedTask;
    }

    private static bool AttemptOutputCaching(OutputCacheContext context)
    {
        // Check if the current request fulfills the requirements to be cached.

        var request = context.HttpContext.Request;

        // Verify the method.
        if (!HttpMethods.IsGet(request.Method) && !HttpMethods.IsHead(request.Method))
        {
            return false;
        }

        // Verify the existence of authorization headers.
        // if (!StringValues.IsNullOrEmpty(request.Headers.Authorization) ||
        //     request.HttpContext.User.Identity?.IsAuthenticated is true)
        // {
        //     return false;
        // }

        return true;
    }
}