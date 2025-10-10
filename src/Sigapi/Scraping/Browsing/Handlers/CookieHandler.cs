using Sigapi.Scraping.Browsing.Sessions;
using ISession = Sigapi.Scraping.Browsing.Sessions.ISession;

namespace Sigapi.Scraping.Browsing.Handlers;

internal sealed class CookieHandler : DelegatingHandler
{
    private const string CookieHeader = "Cookie";
    private const string SetCookieHeader = "Set-Cookie";

    public static readonly HttpRequestOptionsKey<ISession?> SessionKey = new("Session");

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.RequestUri is null
            || !request.Options.TryGetValue(SessionKey, out var session)
            || session is null)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        if (session.IsExpired)
        {
            throw new SessionExpiredException($"Session '{session.Id}' has expired.");
        }

        ProcessRequestCookies(request, session);
        
        var response = await base.SendAsync(request, cancellationToken);
        
        ProcessResponseCookies(response, session);

        // Handle session auto-refresh if applicable.
        if (response.IsSuccessStatusCode && session.AutoRefreshLifetime && session is Session concreteSession)
        {
            concreteSession.SetExpirationFromNow();
        }
        
        return response;
    }

    private static void ProcessRequestCookies(HttpRequestMessage request, ISession session)
    {
        var cookies = session.GetCookies(request.RequestUri!);
        if (!string.IsNullOrEmpty(cookies))
        {
            request.Headers.TryAddWithoutValidation(CookieHeader, cookies);
        }
    }

    private static void ProcessResponseCookies(HttpResponseMessage response, ISession session)
    {
        if (response.Headers.TryGetValues(SetCookieHeader, out var cookies))
        {
            session.SetCookies(response.RequestMessage!.RequestUri!, cookies);
        }
    }
}