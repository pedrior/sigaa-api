using Sigaa.Api.Common.Scraping.Configuration;
using Sigaa.Api.Features.Account.Models;
using Sigaa.Api.Features.Account.Scraping;
using Sigaa.Api.Features.Account.Scraping.Configurations;

namespace Sigaa.Api.Features.Account;

internal static class DependencyInjection
{
    public static WebApplicationBuilder AddAccountFeature(this WebApplicationBuilder builder)
    {
        builder.Services.RegisterAuthenticationCoreServices();
        builder.Services.RegisterScrapingModelConfigurations();

        return builder;
    }

    private static void RegisterAuthenticationCoreServices(this IServiceCollection services)
    {
        services.AddTransient<IEnrollmentProvider, EnrollmentProvider>();
        services.AddTransient<IEnrollmentSelector, EnrollmentSelector>();
        
        services.AddTransient<ILoginResponseHandler, CredentialsMismatchHandler>();
        services.AddTransient<ILoginResponseHandler, SingleEnrollmentHandler>();
        services.AddTransient<ILoginResponseHandler, MultipleEnrollmentHandler>();
    }

    private static void RegisterScrapingModelConfigurations(this IServiceCollection services)
    {
        services.AddSingleton<IScrapingModelConfiguration<Enrollment>, EnrollmentConfiguration>();
        services.AddSingleton<IScrapingModelConfiguration<UserEnrollments>, EnrollmentListConfiguration>();
        services.AddSingleton<IScrapingModelConfiguration<LoginForm>, LoginFormConfiguration>();
        services.AddSingleton<IScrapingModelConfiguration<Profile>, ProfileConfiguration>();
    }
}