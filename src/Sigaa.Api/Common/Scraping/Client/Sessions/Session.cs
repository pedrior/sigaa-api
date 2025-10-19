using System.Net;
using System.Text.Json.Serialization;

namespace Sigaa.Api.Common.Scraping.Client.Sessions;

internal sealed class Session : ISession
{
    private static readonly TimeSpan SessionIdleTimeout = TimeSpan.FromMinutes(60);
    private static readonly TimeSpan SessionTimeoutBuffer = TimeSpan.FromSeconds(30);

    private readonly CookieContainer container = new();

    internal Session() : this($"{Guid.CreateVersion7():N}")
    {
    }

    [JsonConstructor]
    internal Session(string id)
    {
        Id = id;
    }

    public string Id { get; }

    public TimeSpan IdleTimeout => SessionIdleTimeout;

    public DateTimeOffset LastAccessed { get; private set; } = DateTimeOffset.UtcNow;

    private DateTimeOffset ExpirationTime => LastAccessed + IdleTimeout - SessionTimeoutBuffer;

    [JsonInclude]
    private CookieCollection Cookies
    {
        get => container.GetAllCookies();
        set => container.Add(value);
    }

    public bool IsExpired() => ExpirationTime < DateTimeOffset.UtcNow;

    public TimeSpan GetRemainingLifetime()
    {
        var remaining = ExpirationTime - DateTimeOffset.UtcNow;
        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }

    public void IncludeCookiesInRequest(HttpRequestMessage request)
    {
        if (request.RequestUri is not { } uri)
        {
            throw new ArgumentException("Request URI cannot be null.", nameof(request));
        }

        var header = container.GetCookieHeader(uri);
        if (string.IsNullOrEmpty(header))
        {
            return;
        }

        // Remove any existing cookies.
        if (request.Headers.Contains("Cookie"))
        {
            request.Headers.Remove("Cookie");
        }

        request.Headers.Add("Cookie", header);
    }

    public void ProcessResponseCookies(HttpResponseMessage response)
    {
        if (response.RequestMessage?.RequestUri is not { } uri)
        {
            throw new ArgumentException("Request URI cannot be null.", nameof(response));
        }

        if (!response.Headers.TryGetValues("Set-Cookie", out var values))
        {
            return;
        }

        foreach (var value in values)
        {
            container.SetCookies(uri, value);
        }
        
        RefreshLastAccessed();
    }
    
    private void RefreshLastAccessed() => LastAccessed = DateTimeOffset.UtcNow;
}