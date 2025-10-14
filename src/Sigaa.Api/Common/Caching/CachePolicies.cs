namespace Sigaa.Api.Common.Caching;

internal static class CachePolicies
{
    internal static class Account
    {
        public const string GetProfile = "acccount:get-profile";
    }

    internal static class Centers
    {
        public const string GetCenter = "centers:get";
        
        public const string ListCenters = "centers:list";
    }

    internal static class Departments
    {
        public const string ListDepartments = "departments:list";
    }
}