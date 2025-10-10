namespace Sigapi.Scraping.Browsing.Sessions;

internal sealed class SessionManager : ISessionManager
{
    private readonly ISessionStore sessionStore;

    public SessionManager(ISessionStore sessionStore)
    {
        this.sessionStore = sessionStore;
    }

    public ISession CreateSession(string? sessionId = null)
    {
        return sessionId is null 
            ? new Session() 
            : new Session(sessionId);
    }
    
    public async Task<ISession> LoadSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        var session = await sessionStore.LoadAsync(sessionId, cancellationToken);
        return session ?? throw new SessionExpiredException("The session has expired.");
    }

    public Task SaveSessionAsync(ISession session, CancellationToken cancellationToken = default)
    {
        (session as Session)?.SetExpirationFromNow();
        return sessionStore.SaveAsync(session, cancellationToken);
    }

    public Task RevokeSessionAsync(string sessionId, CancellationToken cancellationToken = default) => 
        sessionStore.RevokeAsync(sessionId, cancellationToken);

    public async Task<bool> ValidateSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        var session = await sessionStore.LoadAsync(sessionId, cancellationToken);
        return session is { IsExpired: false };
    }
}