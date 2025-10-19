using Sigaa.Api.Common.Scraping.Client.Sessions.Storages;

namespace Sigaa.Api.Common.Scraping.Client.Sessions.Cookies;

internal static class CookiesRequestExtensions
{
    public static HttpRequestMessage SetSessionPolicy(this HttpRequestMessage request, SessionPolicy policy)
    {
        request.Options.Set(CookieRequestKeys.SessionPolicyKey, policy);
        return request;
    }

    public static SessionPolicy GetSessionPolicy(this HttpRequestMessage request)
    {
        return request.Options.TryGetValue(CookieRequestKeys.SessionPolicyKey, out var policy)
            ? policy
            : SessionPolicy.Transient;
    }

    public static string SetRequestedNewSessionId(this HttpRequestMessage request, string sessionId)
    {
        request.Options.Set(CookieRequestKeys.SessionIdKey, sessionId);
        return sessionId;
    }
    
    public static string? GetRequestedNewSessionId(this HttpRequestMessage request)
    {
        return request.Options.TryGetValue(CookieRequestKeys.SessionIdKey, out var sessionId)
            ? sessionId
            : null;
    }

    public static HttpRequestMessage SetMissingSessionBehavior(this HttpRequestMessage request,
        MissingSessionBehavior behavior)
    {
        request.Options.Set(CookieRequestKeys.SessionMissingBehaviorKey, behavior);
        return request;
    }

    public static MissingSessionBehavior GetMissingSessionBehavior(this HttpRequestMessage request)
    {
        return request.Options.TryGetValue(CookieRequestKeys.SessionMissingBehaviorKey, out var behavior)
            ? behavior
            : MissingSessionBehavior.Create;
    }
}