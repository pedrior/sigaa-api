using Sigaa.Api.Common.Scraping.Configuration;
using Sigaa.Api.Features.Departments.Models;
using Sigaa.Api.Features.Departments.Scraping.Configurations;

namespace Sigaa.Api.Features.Departments;

internal static class DependencyInjection
{
    public static WebApplicationBuilder AddDepartmentsFeature(this WebApplicationBuilder builder)
    {
        builder.Services.RegisterScrapingModelConfigurations();
        
        return builder;
    }
    
    private static void RegisterScrapingModelConfigurations(this IServiceCollection services)
    {
        services.AddSingleton<IScrapingModelConfiguration<Department>, DepartmentConfiguration>();
        services.AddSingleton<IScrapingModelConfiguration<DepartmentCenter>, DepartmentCenterConfiguration>();
        services.AddSingleton<IScrapingModelConfiguration<DepartmentDetails>, DepartmentDetailsConfiguration>();
        services.AddSingleton<IScrapingModelConfiguration<DepartmentListingForm>, DepartmentListingFormConfiguration>();
        services.AddSingleton<IScrapingModelConfiguration<DepartmentEntry>, DepartmentEntryConfiguration>();
    }
}