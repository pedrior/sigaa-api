namespace Sigaa.Api.Common.RateLimiting;

internal static class RateLimiterPolicies
{
    public const string Authenticated = "authenticated";
    
    internal static class Account
    {
        public const string SessionManagement = "account:session-management";
    }
}