using System.Net;
using System.Text.Json.Serialization;

namespace Sigapi.Scraping.Browsing.Sessions;

[JsonConverter(typeof(SessionSerializer))]
internal sealed class Session : ISession
{
    public static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(60);
    private static readonly TimeSpan ClockSkew = TimeSpan.FromSeconds(10);

    private readonly CookieContainer cookieContainer = new();

    public Session()
    {
        SetExpirationFromNow();
    }

    public Session(string id) : this() => Id = id;

    internal Session(string id,
        DateTimeOffset createdAt,
        DateTimeOffset expiresAt,
        bool autoRefresh,
        IEnumerable<Cookie> cookies)
    {
        Id = id;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
        AutoRefreshLifetime = autoRefresh;

        foreach (var cookie in cookies)
        {
            cookieContainer.Add(cookie);
        }
    }

    public string Id { get; } = $"{Guid.NewGuid():N}";

    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;

    public DateTimeOffset ExpiresAt { get; private set; }

    public bool IsExpired => DateTimeOffset.UtcNow + ClockSkew >= ExpiresAt;

    public bool AutoRefreshLifetime { get; set; } = true;

    public void SetCookies(Uri target, IEnumerable<string> cookies)
    {
        foreach (var cookie in cookies)
        {
            cookieContainer.SetCookies(target, cookie);
        }
    }

    public string GetCookies(Uri target) => cookieContainer.GetCookieHeader(target);

    public IEnumerable<Cookie> ListCookies() => cookieContainer.GetAllCookies();

    internal void SetExpirationFromNow() => ExpiresAt = DateTimeOffset.UtcNow.Add(Lifetime);
}