namespace Sigaa.Api.Common.Scraping.Browsing.Sessions.Strategies;

internal interface ISessionStrategy
{
    Task<ISession> GetSessionAsync(CancellationToken cancellationToken = default);
}