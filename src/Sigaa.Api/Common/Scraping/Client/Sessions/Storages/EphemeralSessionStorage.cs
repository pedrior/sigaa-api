namespace Sigaa.Api.Common.Scraping.Client.Sessions.Storages;

internal sealed class EphemeralSessionStorage : SessionStorage
{
    private const string SessionKey = "Scraping:Client:Session";

    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ILogger<EphemeralSessionStorage> logger;

    public EphemeralSessionStorage(IHttpContextAccessor httpContextAccessor, ILogger<EphemeralSessionStorage> logger)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.logger = logger;
    }

    public override ValueTask<ISession> GetSessionAsync(CancellationToken cancellation = default)
    {
        var httpContext = GetHttpContext();

        // Attempts to return an existing session.
        if (httpContext.Items.TryGetValue(SessionKey, out var found) && found is ISession session)
        {
            return new ValueTask<ISession>(session);
        }

        logger.LogInformation(
            "Creating new ephemeral session for HTTP request {TraceIdentifier}",
            httpContext.TraceIdentifier);
        
        // No session found, creates a new one.
        var newSession = new Session();
        httpContext.Items[SessionKey] = newSession;

        return new ValueTask<ISession>(newSession);
    }
    
    private HttpContext GetHttpContext()
    {
        return httpContextAccessor.HttpContext ??
               throw new InvalidOperationException("No HTTP context available.");
    }
}