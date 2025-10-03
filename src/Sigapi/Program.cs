using Serilog;
using Sigapi;

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