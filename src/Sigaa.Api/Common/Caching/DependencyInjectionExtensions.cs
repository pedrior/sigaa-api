using Microsoft.AspNetCore.OutputCaching;

namespace Sigaa.Api.Common.Caching;

internal static class DependencyInjection
{
    public static WebApplicationBuilder AddCachingServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddMemoryCache();

        builder.Services.AddOutputCache(options =>
        {
            ConfigureBaselineCache(options);
            ConfigureAccountCachePolicies(options);
            ConfigureCentersCachePolicies(options);
            ConfigureDepartmentsCachePolicies(options);
        });
        
        return builder;
    }

    private static void ConfigureBaselineCache(OutputCacheOptions options) => 
        options.AddBasePolicy(b => b.AddPolicy<BaseCachePolicy>(), excludeDefaultPolicy: true);

    private static void ConfigureAccountCachePolicies(OutputCacheOptions options)
    {
        options.AddPolicy(
            name: CachePolicies.Account.GetProfile,
            builder => builder.Expire(TimeSpan.FromMinutes(10))
                .VaryByUserClaim(JwtRegisteredClaimNames.Sid),
            excludeDefaultPolicy: true);
    }
    
    private static void ConfigureCentersCachePolicies(OutputCacheOptions options)
    {
        options.AddPolicy(
            name: CachePolicies.Centers.ListCenters,
            builder => builder.Expire(TimeSpan.FromHours(24)),
            excludeDefaultPolicy: true);
        
        options.AddPolicy(
            name: CachePolicies.Centers.GetCenter,
            builder => builder.Expire(TimeSpan.FromHours(12))
                .SetVaryByRouteValue("idOrSlug"),
            excludeDefaultPolicy: true);
    }
    
    private static void ConfigureDepartmentsCachePolicies(OutputCacheOptions options)
    {
        options.AddPolicy(
            name: CachePolicies.Departments.ListDepartments,
            builder => builder.Expire(TimeSpan.FromHours(24)),
            excludeDefaultPolicy: true);
    }
}