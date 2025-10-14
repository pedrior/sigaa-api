namespace Sigaa.Api.Features.Departments.Models;

[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed class DepartmentListingForm
{
    private const string AllCentersId = "0";
    
    public string Action { get; set; } = string.Empty;

    public Dictionary<string, string> Data { get; set; } = new();

    public Dictionary<string, string> BuildSubmissionData(string? centerId = null)
    {
        var data = new Dictionary<string, string>(Data)
        {
            ["form:programas"] = centerId ?? AllCentersId
        };

        data.Remove("form:cancelar");
        
        return data;
    }
}