namespace Sigapi.Features.Departments.Contracts;

[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed record DepartmentResponse
{
    public string Id { get; init; } = null!;
    
    public string Name { get; init; } = null!;
}