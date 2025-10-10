namespace Sigapi.Scraping.Browsing.Sessions;

internal interface ISessionManager
{
    ISession CreateSession(string? sessionId = null);
    
    Task<ISession> LoadSessionAsync(string sessionId, CancellationToken cancellationToken = default);
    
    Task SaveSessionAsync(ISession session, CancellationToken cancellationToken = default);
    
    Task RevokeSessionAsync(string sessionId, CancellationToken cancellationToken = default);
    
    Task<bool> ValidateSessionAsync(string sessionId, CancellationToken cancellationToken = default);
}