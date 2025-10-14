using Sigaa.Api.Common.Scraping.Configuration;
using Sigaa.Api.Features.Centers.Models;
using Sigaa.Api.Features.Centers.Scraping.Configurations;

namespace Sigaa.Api.Features.Centers;

internal static class DependencyInjection
{
    public static WebApplicationBuilder AddCentersFeature(this WebApplicationBuilder builder)
    {
        builder.Services.RegisterScrapingModelConfigurations();

        return builder;
    }

    private static void RegisterScrapingModelConfigurations(this IServiceCollection services)
    {
        services.AddSingleton<IScrapingModelConfiguration<Center>, CenterConfiguration>();
        services.AddSingleton<IScrapingModelConfiguration<CenterDetails>, CenterDetailsConfiguration>();
        services.AddSingleton<IScrapingModelConfiguration<Department>, DepartmentConfiguration>();
        services.AddSingleton<IScrapingModelConfiguration<GraduateProgram>, GraduateProgramConfiguration>();
        services.AddSingleton<IScrapingModelConfiguration<UndergraduateProgram>, UndergraduateProgramConfiguration>();
        services.AddSingleton<IScrapingModelConfiguration<Research>, ResearchConfiguration>();
        services.AddSingleton<IScrapingModelConfiguration<ResearchCoordinator>, ResearchCoordinatorConfiguration>();
    }
}