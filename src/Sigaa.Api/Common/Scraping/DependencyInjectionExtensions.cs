using System.Net;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Polly.Timeout;
using Sigaa.Api.Common.Scraping.Client;
using Sigaa.Api.Common.Scraping.Client.Redirects;
using Sigaa.Api.Common.Scraping.Client.Sessions;
using Sigaa.Api.Common.Scraping.Client.Sessions.Cookies;
using Sigaa.Api.Common.Scraping.Client.Sessions.Storages;
using Sigaa.Api.Common.Scraping.Configuration;
using Sigaa.Api.Common.Scraping.Converters;
using Sigaa.Api.Common.Scraping.Document;
using Sigaa.Api.Common.Scraping.Strategies;

namespace Sigaa.Api.Common.Scraping;

internal static class ScrapingServicesExtensions
{
    public static WebApplicationBuilder AddScrapingServices(this WebApplicationBuilder builder)
    {
        builder.Services.RegisterCoreScrapingServices();
        builder.Services.RegisterScrapingModelServices();
        builder.Services.RegisterClientServices();

        return builder;
    }

    private static void RegisterCoreScrapingServices(this IServiceCollection services)
    {
        services.AddSingleton<IScrapingEngine, ScrapingEngine>();
        services.AddSingleton<IConversionService, ConversionService>();
        services.AddSingleton<IHtmlParser, HtmlParser>();
    }

    private static void RegisterScrapingModelServices(this IServiceCollection services)
    {
        services.AddTransient(typeof(IModelScraper<>), typeof(ModelScraper<>));
        services.AddSingleton<IModelScraperFactory, ModelScraperFactory>();
        services.AddSingleton<IScrapingModelConfigurationProvider, ScrapingModelConfigurationProvider>();

        services.AddSingleton<IPropertyScraper, ValuePropertyScraper>();
        services.AddSingleton<IPropertyScraper, ObjectPropertyScraper>();
        services.AddSingleton<IPropertyScraper, CollectionPropertyScraper>();
        services.AddSingleton<IPropertyScraper, DictionaryPropertyScraper>();
    }

    private static void RegisterClientServices(this IServiceCollection services)
    {
        services.AddOptionsWithValidateOnStart<FetcherClientOptions>()
            .BindConfiguration(FetcherClientOptions.SectionName)
            .ValidateDataAnnotations();

        services.AddTransient<ISessionStorageResolver, SessionStorageResolver>();
        services.AddKeyedScoped<ISessionStorage, PersistentSessionStorage>(SessionPolicy.Persistent);
        services.AddKeyedScoped<ISessionStorage, EphemeralSessionStorage>(SessionPolicy.Ephemeral);
        services.AddKeyedSingleton<ISessionStorage, TransientSessionStorage>(SessionPolicy.Transient);

        services.AddScoped<ISessionRevoker, SessionRevoker>();
        services.AddScoped<ISessionDetailsAccessor, SessionDetailsAccessor>();

        services.AddTransient<CookieHandler>();
        services.AddTransient<RedirectHandler>();

        services.AddHttpClient<IFetcher, Fetcher>()
            .ConfigureHttpClient(client =>
            {
                // Handled by resilience policies.
                client.Timeout = Timeout.InfiniteTimeSpan;
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                UseCookies = false,
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.All
            })
            .AddHttpMessageHandler<RedirectHandler>()
            .AddHttpMessageHandler<CookieHandler>()
            .AddStandardResilienceHandler(options =>
            {
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(90);
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(15);

                ConfigureScrapingClientRetryPolicy(options.Retry);
                ConfigureScrapingClientCircuitBreakerPolicy(options.CircuitBreaker);
            });
    }

    private static void ConfigureScrapingClientRetryPolicy(HttpRetryStrategyOptions options)
    {
        options.MaxRetryAttempts = 5;
        options.Delay = TimeSpan.FromSeconds(2);
        options.BackoffType = DelayBackoffType.Exponential;
        options.UseJitter = true;

        // Use the 'Retry-After' header if the server provides it.
        options.ShouldRetryAfterHeader = true;

        // Handle transient errors and specific HTTP status codes.
        options.ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
            .Handle<HttpRequestException>()
            .Handle<TimeoutRejectedException>()
            .HandleResult(response => response.StatusCode is >= HttpStatusCode.InternalServerError
                or HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests);
    }

    private static void ConfigureScrapingClientCircuitBreakerPolicy(HttpCircuitBreakerStrategyOptions options)
    {
        options.ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
            .Handle<HttpRequestException>()
            .Handle<TimeoutRejectedException>()
            .HandleResult(message => message.StatusCode is >= HttpStatusCode.InternalServerError
                or HttpStatusCode.RequestTimeout);

        options.FailureRatio = 0.4;
        options.MinimumThroughput = 20;

        // The duration over which failure rates are tracked.
        options.SamplingDuration = TimeSpan.FromSeconds(30);

        // The duration of the circuit will stay open before transitioning to half-open.
        options.BreakDuration = TimeSpan.FromSeconds(60);
    }
}