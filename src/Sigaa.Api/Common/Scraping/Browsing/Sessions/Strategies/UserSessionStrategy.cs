using Sigaa.Api.Common.Security;

namespace Sigaa.Api.Common.Scraping.Browsing.Sessions.Strategies;

internal sealed class UserSessionStrategy : ISessionStrategy
{
    private readonly IUserContext userContext;
    private readonly ISessionManager sessionManager;
    
    public UserSessionStrategy(IUserContext userContext, ISessionManager sessionManager)
    {
        this.userContext = userContext;
        this.sessionManager = sessionManager;
    }
    
    public async Task<ISession> GetSessionAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var sessionId = userContext.SessionId;
        if (string.IsNullOrEmpty(sessionId))
        {
            throw new InvalidOperationException("User is not authenticated or does not have a session.");
        }

        // Optimistically, load the session. This will throw a SessionExpiredException if the session is expired.
        return await sessionManager.LoadSessionAsync(sessionId, cancellationToken);
    }
}