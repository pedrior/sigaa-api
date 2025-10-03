namespace Sigapi.Features.Centers.Contracts;

[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed record ResearchResponse
{
    public string Name { get; init; } = string.Empty;
    
    public ResearchCoordinatorResponse Coordinator { get; init; } = new();
}