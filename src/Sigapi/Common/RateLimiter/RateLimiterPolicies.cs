namespace Sigapi.Common.RateLimiter;

internal static class RateLimiterPolicies
{
    public const string Authenticated = "authenticated";
    
    internal static class Account
    {
        public const string SessionManagement = "account:session-management";
    }
}