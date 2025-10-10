namespace Sigapi.Scraping.Browsing.Sessions.Strategies;

internal sealed class AnonymousSessionStrategy : ISessionStrategy
{
    private readonly ISessionManager sessionManager;

    public AnonymousSessionStrategy(ISessionManager sessionManager)
    {
        this.sessionManager = sessionManager;
    }

    public Task<ISession> GetSessionAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        return Task.FromResult(sessionManager.CreateSession());
    }
}