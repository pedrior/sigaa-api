using Serilog;
using Sigapi;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting up");
    
    var builder = WebApplication.CreateBuilder(args);

    builder.AddLogging();

    builder.Services.AddApiServices(builder.Configuration);

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
    app.UseScalarUI();

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