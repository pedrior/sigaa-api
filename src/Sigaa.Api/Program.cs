using Serilog;
using Sigaa.Api;
using Sigaa.Api.Common.Caching;
using Sigaa.Api.Common.OpenApi;
using Sigaa.Api.Common.RateLimiting;
using Sigaa.Api.Common.Scraping;
using Sigaa.Api.Common.Security;
using Sigaa.Api.Features.Account;
using Sigaa.Api.Features.Centers;
using Sigaa.Api.Features.Departments;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting up");

    var builder = WebApplication.CreateBuilder(args)
        .AddLogging()
        .AddApiServices()
        .AddRateLimitingServices()
        .AddSecurityServices()
        .AddCachingServices()
        .AddOpenApiServices()
        .AddScrapingServices()
        .AddAccountFeature()
        .AddCentersFeature()
        .AddDepartmentsFeature();

    var app = builder.Build();

    app.UseForwardedHeaders();
    app.UseSerilogRequestLogging();
    app.UseExceptionHandler();
    app.UseHttpsRedirection();
    app.UseStatusCodePages();
    app.UseRateLimiter();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseOutputCache();
    app.UseScalarUi();

    app.MapEndpoints();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}