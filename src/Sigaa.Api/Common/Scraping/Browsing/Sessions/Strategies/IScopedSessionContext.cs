namespace Sigaa.Api.Common.Scraping.Browsing.Sessions.Strategies;

internal interface IScopedSessionContext
{
    ISession? Session { get; set; }
    
    object SyncLock { get; }
}