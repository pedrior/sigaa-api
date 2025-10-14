namespace Sigaa.Api.Common.Scraping.Browsing.Sessions.Strategies;

internal sealed class ContextualSessionStrategy : ISessionStrategy
{
    private readonly Lock syncLock = new();
    
    private readonly ISessionManager sessionManager;
    private readonly IScopedSessionContext scopedSessionContext;

    public ContextualSessionStrategy(ISessionManager sessionManager, IScopedSessionContext scopedSessionContext)
    {
        this.sessionManager = sessionManager;
        this.scopedSessionContext = scopedSessionContext;
    }

    public Task<ISession> GetSessionAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (syncLock.EnterScope())
        {
            scopedSessionContext.Session ??= sessionManager.CreateSession();
        }

        // The lock ensures the session is not null at this point.
        return Task.FromResult(scopedSessionContext.Session);
    }
}