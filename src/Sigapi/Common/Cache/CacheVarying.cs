using System.Security.Claims;

namespace Sigapi.Common.Cache;

internal static class CacheVarying
{
    public static ValueTask<KeyValuePair<string, string>> VaryBySession(HttpContext context, CancellationToken _)
    {
        const string claimType = JwtRegisteredClaimNames.Sid;
        var claimValue = context.User.FindFirstValue(claimType);

        return string.IsNullOrEmpty(claimValue)
            ? throw new InvalidOperationException("The session claim is not present.")
            : ValueTask.FromResult(new KeyValuePair<string, string>(claimType, claimValue));
    }
}