namespace Sigapi.Features.Centers.Contracts;

[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed record ProgramResponse
{
    public string Id { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public ProgramType Type { get; init; }

    public string? City { get; init; }
        
    public ProgramModality? Modality { get; init; }

    public string? Coordinator { get; init; }
}