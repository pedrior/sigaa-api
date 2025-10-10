using System.Net;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.Timeout;
using Scalar.AspNetCore;
using Sigapi.Common.Cache;
using Sigapi.Common.Exceptions;
using Sigapi.Common.OpenApi;
using Sigapi.Common.RateLimiter;
using Sigapi.Common.Security;
using Sigapi.Common.Security.Tokens;
using Sigapi.Features.Account.Scraping;
using Sigapi.Scraping.Browsing;
using Sigapi.Scraping.Browsing.Handlers;
using Sigapi.Scraping.Browsing.Sessions;
using Sigapi.Scraping.Browsing.Sessions.Strategies;
using Sigapi.Scraping.Configuration;
using Sigapi.Scraping.Converters;
using Sigapi.Scraping.Document;
using Sigapi.Scraping.Engine;
using Sigapi.Scraping.Engine.Strategies;

namespace Sigapi;

public static class Services
{
    private static readonly Assembly ThisAssembly = typeof(Services).Assembly;

    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureSerializerOptions();
        services.ConfigureProblemDetails();
        services.ConfigureForwardedHeaders();

        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddSingleton<ProblemDetailsFactory, DefaultProblemDetailsFactory>();

        services.AddOpenApi();

        services.AddCache();

        services.AddValidation();

        services.AddSecurity(configuration);

        services.AddRateLimiter();

        services.AddScraping();

        return services;
    }

    private static void ConfigureSerializerOptions(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(
                options.SerializerOptions.PropertyNamingPolicy));
        });
    }

    private static void ConfigureProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions.Add("requestId", context.HttpContext.TraceIdentifier);
            };
        });
    }

    private static void ConfigureForwardedHeaders(this IServiceCollection services) =>
        services.Configure<ForwardedHeadersOptions>(options => options.ForwardedHeaders = ForwardedHeaders.All);

    private static void AddOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddScalarTransformers();

            options.AddDocumentTransformer<ApiInfoDocumentTransformer>();
            options.AddDocumentTransformer<TagsEnhancerDocumentTransformer>();
            options.AddDocumentTransformer<DynamicBaseServerDocumentTransformer>();
            options.AddDocumentTransformer<BearerSecuritySchemeDocumentTransformer>();
        });
    }

    private static void AddRateLimiter(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // Limit requests to 100 per minute per client IP.
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(clientIp, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1)
                });
            });

            // Policy for session management (login and logout) - limit to 5 requests per minute per client IP.
            options.AddPolicy(RateLimiterPolicies.Account.SessionManagement, context =>
            {
                var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetTokenBucketLimiter(clientIp, _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 5,
                    TokensPerPeriod = 5,
                    ReplenishmentPeriod = TimeSpan.FromMinutes(1)
                });
            });

            // Policy for account-related endpoints - limit to 25 requests per minute per client IP.
            options.AddPolicy(RateLimiterPolicies.Authenticated, context =>
            {
                var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetTokenBucketLimiter(clientIp, _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 25,
                    TokensPerPeriod = 25,
                    ReplenishmentPeriod = TimeSpan.FromMinutes(1)
                });
            });

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (rejectedContext, cancellationToken) =>
            {
                if (rejectedContext.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    rejectedContext.HttpContext.Response.Headers.RetryAfter = $"{retryAfter.TotalSeconds}";
                }

                var problemDetailsFactory = rejectedContext.HttpContext.RequestServices
                    .GetRequiredService<ProblemDetailsFactory>();

                var problemDetails = problemDetailsFactory.CreateProblemDetails(
                    rejectedContext.HttpContext,
                    statusCode: StatusCodes.Status429TooManyRequests,
                    detail: "You have exceeded the allowed number of requests.");

                await rejectedContext.HttpContext.Response.WriteAsJsonAsync(
                    problemDetails,
                    cancellationToken: cancellationToken);
            };
        });
    }

    private static void AddValidation(this IServiceCollection services) =>
        services.AddValidatorsFromAssembly(ThisAssembly, includeInternalTypes: true);

    private static void AddCache(this IServiceCollection services)
    {
        services.AddMemoryCache();

        services.AddOutputCache(options =>
        {
            options.AddBasePolicy(b => b.AddPolicy<BaseCachePolicy>(), excludeDefaultPolicy: true);

            // Cache policies for account-related endpoints.
            options.AddPolicy(
                name: CachePolicies.Account.GetProfile,
                builder => builder.Expire(TimeSpan.FromMinutes(10))
                    .VaryByUserClaim(JwtRegisteredClaimNames.Sid),
                excludeDefaultPolicy: true);

            // Cache policies for centers-related endpoints.
            options.AddPolicy(
                name: CachePolicies.Centers.GetCenter,
                builder => builder.Expire(TimeSpan.FromHours(12))
                    .SetVaryByRouteValue("idOrSlug"),
                excludeDefaultPolicy: true);

            options.AddPolicy(
                name: CachePolicies.Centers.ListCenters,
                builder => builder.Expire(TimeSpan.FromHours(24)),
                excludeDefaultPolicy: true);
        });
    }

    private static void AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDataProtection();
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        services.AddTransient<ISecurityTokenProvider, SecurityTokenProvider>();

        var jwtConfig = configuration.GetRequiredSection("Jwt");
        services.AddOptionsWithValidateOnStart<SecurityTokenOptions>()
            .Bind(jwtConfig)
            .ValidateDataAnnotations();

        var securityTokenOptions = jwtConfig.Get<SecurityTokenOptions>()!;
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuer = !string.IsNullOrEmpty(securityTokenOptions.Issuer),
                    ValidIssuer = securityTokenOptions.Issuer,
                    ValidateAudience = !string.IsNullOrEmpty(securityTokenOptions.Audience),
                    ValidAudience = securityTokenOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(securityTokenOptions.Key))
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var sid = context.Principal!.Claims.First(c => c.Type is JwtRegisteredClaimNames.Sid);
                        var sessionManager = context.HttpContext.RequestServices.GetRequiredService<ISessionManager>();

                        if (!await sessionManager.ValidateSessionAsync(sid.Value))
                        {
                            context.Fail("Session is no longer valid. Please login again.");
                        }
                    }
                };
            });

        services.AddAuthorization();
    }

    private static void AddScraping(this IServiceCollection services)
    {
        // Session-related services.
        services.AddSingleton<ISessionManager, SessionManager>();
        services.AddSingleton<ISessionStore, SessionStore>();
        services.AddTransient<AnonymousSessionStrategy>();
        services.AddTransient<ContextualSessionStrategy>();
        services.AddTransient<UserSessionStrategy>();
        services.AddTransient<ISessionStrategyProvider, SessionStrategyProvider>();
        services.AddScoped<IScopedSessionContext, ScopedSessionContext>();

        // Http delegating handlers.
        services.AddTransient<CookieHandler>();
        services.AddTransient<RedirectHandler>();

        // Core Scraping Services.
        services.AddSingleton<IScrapingEngine, ScrapingEngine>();
        services.AddSingleton<IConversionService, ConversionService>();

        services.AddSingleton<IHtmlParser, HtmlParser>();

        services.AddTransient(typeof(IModelScraper<>), typeof(ModelScraper<>));
        services.AddSingleton<IModelScraperFactory, ModelScraperFactory>();
        services.AddSingleton<IScrapingModelConfigurationProvider, ScrapingModelConfigurationProvider>();

        services.AddSingleton<IPropertyScraper, ValuePropertyScraper>();
        services.AddSingleton<IPropertyScraper, ObjectPropertyScraper>();
        services.AddSingleton<IPropertyScraper, CollectionPropertyScraper>();
        services.AddSingleton<IPropertyScraper, DictionaryPropertyScraper>();

        services.AddHttpClient<IResourceLoader, ResourceLoader>()
            .ConfigureHttpClient(client =>
            {
                // Handled by resilience policies.
                client.Timeout = Timeout.InfiniteTimeSpan;

                client.BaseAddress = new Uri("https://sigaa.ufpb.br");

                client.DefaultRequestHeaders.Add("Accept", "*/*");
                client.DefaultRequestHeaders.Add("Accept-Language", "pt-BR");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                client.DefaultRequestHeaders.Add("User-Agent", "Sigapi/1.0");
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

        services.Scan(selector => selector.FromAssemblies(ThisAssembly)
            .AddClasses(filter => filter.AssignableTo(typeof(IScrapingModelConfiguration<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithSingletonLifetime());

        // Account-related services.
        services.AddTransient<IEnrollmentProvider, EnrollmentProvider>();
        services.AddTransient<IEnrollmentSelector, EnrollmentSelector>();
        services.AddTransient<ILoginResponseHandler, CredentialsMismatchHandler>();
        services.AddTransient<ILoginResponseHandler, SingleEnrollmentHandler>();
        services.AddTransient<ILoginResponseHandler, MultipleEnrollmentHandler>();
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