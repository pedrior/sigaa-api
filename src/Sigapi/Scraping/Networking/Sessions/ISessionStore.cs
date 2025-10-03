namespace Sigapi.Scraping.Networking.Sessions;

internal interface ISessionStore
{
    Task SaveAsync(ISession session, CancellationToken cancellationToken = default);
    
    Task<ISession?> LoadAsync(string sessionId, CancellationToken cancellationToken = default);
    
    Task RevokeAsync(string sessionId, CancellationToken cancellationToken = default);
}