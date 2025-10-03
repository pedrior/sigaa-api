namespace Sigapi.Common.Cache;

internal static class CachePolicies
{
    internal static class Account
    {
        public const string GetProfile = "acccount:get-profile";
    }

    internal static class Centers
    {
        public const string GetCenter = "centers:get-center";
        
        public const string ListCenters = "centers:list-centers";
    }
}