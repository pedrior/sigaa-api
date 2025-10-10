namespace Sigapi.Scraping.Browsing.Sessions;

internal interface ISession
{
    string Id { get; }
    
    bool IsExpired { get; }
    
    bool AutoRefreshLifetime { get; set; }
    
    DateTimeOffset CreatedAt { get; }
    
    DateTimeOffset ExpiresAt { get; }

    internal void SetCookies(Uri target, IEnumerable<string> cookies);
    
    internal string GetCookies(Uri target);
}