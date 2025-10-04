namespace Sigapi.Features.Departments.Models;

[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed class DepartmentCenter
{
    public string Name { get; set; } = string.Empty;
    
    public string Slug { get; set; } = string.Empty;
    
    public string Acronym { get; set; } = string.Empty;
}