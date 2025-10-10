namespace Sigapi.Scraping.Browsing.Sessions.Strategies;

internal interface ISessionStrategy
{
    Task<ISession> GetSessionAsync(CancellationToken cancellationToken = default);
}