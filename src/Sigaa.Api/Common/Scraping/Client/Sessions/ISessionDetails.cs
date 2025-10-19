namespace Sigaa.Api.Common.Scraping.Client.Sessions;

internal interface ISessionDetails
{
    string Id { get; }
    
    TimeSpan IdleTimeout { get; }
    
    DateTimeOffset LastAccessed { get; }
    
    bool IsExpired();
    
    TimeSpan GetRemainingLifetime();
}