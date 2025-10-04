namespace Sigapi.Features.Departments.Contracts;

[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed record DepartmentGroupResponse
{
    public string CenterName { get; init; } = null!;
    
    public string CenterSlug { get; init; } = null!;
    
    public IEnumerable<DepartmentResponse> Departments { get; init; } = [];
}