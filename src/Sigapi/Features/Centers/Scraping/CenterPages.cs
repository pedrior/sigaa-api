namespace Sigapi.Features.Centers.Scraping;

internal static class CenterPages
{
    public const string CenterList = "sigaa/public/centro/lista.jsf";
    private const string CenterFormat = "sigaa/public/centro/portal.jsf?id={0}";
    private const string DepartmentsFormat = "sigaa/public/centro/lista_departamentos.jsf?id={0}";
    private const string UndergraduateProgramsFormat = "sigaa/public/centro/lista_cursos.jsf?id={0}";
    private const string PostgraduateProgramsFormat = "sigaa/public/centro/lista_programas.jsf?id={0}";
    private const string ResearchesFormat = "sigaa/public/centro/bases_pesquisa.jsf?id={0}";
    
    public static string GetCenter(string id) => string.Format(CenterFormat, id);
    
    public static string GetDepartments(string id) => string.Format(DepartmentsFormat, id);
    
    public static string GetUndergraduatePrograms(string id) => string.Format(UndergraduateProgramsFormat, id);
    
    public static string GetPostgraduatePrograms(string id) => string.Format(PostgraduateProgramsFormat, id);
    
    public static string GetResearches(string id) => string.Format(ResearchesFormat, id);
}