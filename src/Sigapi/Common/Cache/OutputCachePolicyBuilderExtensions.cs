using System.Security.Claims;
using Microsoft.AspNetCore.OutputCaching;

namespace Sigapi.Common.Cache;

internal static class OutputCachePolicyBuilderExtensions
{
    public static OutputCachePolicyBuilder VaryByUserClaim(this OutputCachePolicyBuilder builder, string claimType)
    {
        builder.VaryByValue((context, _) =>
        {
            var value = context.User.FindFirstValue(claimType);
            return string.IsNullOrEmpty(value)
                ? throw new InvalidOperationException($"The claim '{claimType}' was not found in the current user.")
                : ValueTask.FromResult(new KeyValuePair<string, string>(claimType, value));
        });

        return builder;
    }
}