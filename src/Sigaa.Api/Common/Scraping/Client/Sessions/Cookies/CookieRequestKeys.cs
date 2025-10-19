using Sigaa.Api.Common.Scraping.Client.Sessions.Storages;

namespace Sigaa.Api.Common.Scraping.Client.Sessions.Cookies;

internal static class CookieRequestKeys
{
    public static readonly HttpRequestOptionsKey<SessionPolicy> SessionPolicyKey = new("SessionPolicy");
 
    public static readonly HttpRequestOptionsKey<string> SessionIdKey = new("SessionId");
    
    public static readonly HttpRequestOptionsKey<MissingSessionBehavior> SessionMissingBehaviorKey = 
        new("SessionMissingBehavior");
}