using System.Security.Claims;

namespace Sigaa.Api.Common.Scraping.Client.Sessions.Storages;

internal static class PersistentSessionStorageHelper
{
    public static string? GetSessionId(HttpContext? httpContext)
    {
        return httpContext is null 
            ? throw new InvalidOperationException("No HTTP context available.") 
            : httpContext.User.FindFirstValue(PersistentSessionStorage.SessionIdClaimType);
    }
}