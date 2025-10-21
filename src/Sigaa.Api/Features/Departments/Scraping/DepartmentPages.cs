namespace Sigaa.Api.Features.Departments.Scraping;

internal static class DepartmentPages
{
    public const string Listing = "/sigaa/public/departamento/lista.jsf";

    private const string DepartmentUrlFormat = "/sigaa/public/departamento/portal.jsf?id={0}";
    
    public static string GetDepartmentPageUrl(string code) => string.Format(DepartmentUrlFormat, code);
}