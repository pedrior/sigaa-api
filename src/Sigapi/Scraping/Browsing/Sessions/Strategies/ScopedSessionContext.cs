namespace Sigapi.Scraping.Browsing.Sessions.Strategies;

internal sealed class ScopedSessionContext : IScopedSessionContext
{
    public ISession? Session { get; set; }
    
    public object SyncLock { get; } = new();
}