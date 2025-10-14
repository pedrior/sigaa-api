namespace Sigaa.Api.Common.Scraping.Browsing.Sessions.Strategies;

internal sealed class CustomSessionStrategy : ISessionStrategy
{
    private readonly ISession session;
    
    public CustomSessionStrategy(ISession session)
    {
        this.session = session;
    }
    
    public Task<ISession> GetSessionAsync(CancellationToken cancellationToken = default) => Task.FromResult(session);
}