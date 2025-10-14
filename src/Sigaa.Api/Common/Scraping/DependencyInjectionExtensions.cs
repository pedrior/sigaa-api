using System.Net;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Polly.Timeout;
using Sigaa.Api.Common.Scraping.Browsing;
using Sigaa.Api.Common.Scraping.Browsing.Handlers;
using Sigaa.Api.Common.Scraping.Browsing.Sessions;
using Sigaa.Api.Common.Scraping.Browsing.Sessions.Strategies;
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
        builder.Services.RegisterSessionManagement();
        builder.Services.RegisterHttpDelegatingHandlers();
        builder.Services.RegisterResourceLoaderClient();

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

    private static void RegisterSessionManagement(this IServiceCollection services)
    {
        services.AddSingleton<ISessionManager, SessionManager>();
        services.AddSingleton<ISessionStore, SessionStore>();
        services.AddTransient<AnonymousSessionStrategy>();
        services.AddTransient<ContextualSessionStrategy>();
        services.AddTransient<UserSessionStrategy>();
        services.AddTransient<ISessionStrategyProvider, SessionStrategyProvider>();
        services.AddScoped<IScopedSessionContext, ScopedSessionContext>();
    }

    private static void RegisterHttpDelegatingHandlers(this IServiceCollection services)
    {
        services.AddTransient<CookieHandler>();
        services.AddTransient<RedirectHandler>();
    }

    private static void RegisterResourceLoaderClient(this IServiceCollection services)
    {
        services.AddHttpClient<IResourceLoader, ResourceLoader>()
            .ConfigureHttpClient(client =>
            {
                // Handled by resilience policies.
                client.Timeout = Timeout.InfiniteTimeSpan;

                client.BaseAddress = new Uri("https://sigaa.ufpb.br");

                client.DefaultRequestHeaders.Add("Accept", "*/*");
                client.DefaultRequestHeaders.Add("Accept-Language", "pt-BR");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                client.DefaultRequestHeaders.Add("User-Agent", "SIGAA API/1.0");
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