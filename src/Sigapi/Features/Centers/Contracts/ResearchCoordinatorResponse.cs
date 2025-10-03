namespace Sigapi.Features.Centers.Contracts;

[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed record ResearchCoordinatorResponse
{
    public string Id { get; init; } = string.Empty;
    
    public string Name { get; init; } = string.Empty;
}