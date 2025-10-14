using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Primitives;

namespace Sigaa.Api.Common.Caching;

internal sealed class BaseCachePolicy : IOutputCachePolicy
{
    public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        var cacheRequestAllowed = ShouldCacheRequest(context);
        
        context.EnableOutputCaching = true;
        context.AllowCacheLookup = cacheRequestAllowed;
        context.AllowCacheStorage = cacheRequestAllowed;
        context.AllowLocking = true;

        // Vary by any query by default.
        context.CacheVaryByRules.QueryKeys = "*";

        return ValueTask.CompletedTask;
    }

    public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        var response = context.HttpContext.Response;
        if (!StringValues.IsNullOrEmpty(response.Headers.SetCookie))
        {
            context.AllowCacheStorage = false;
            return ValueTask.CompletedTask;
        }
        
        if (response.StatusCode is not StatusCodes.Status200OK)
        {
            context.AllowCacheStorage = false;
        }

        return ValueTask.CompletedTask;
    }

    private static bool ShouldCacheRequest(OutputCacheContext context)
    {
        var method = context.HttpContext.Request.Method;
        return HttpMethods.IsGet(method) || HttpMethods.IsHead(method);
    }
}