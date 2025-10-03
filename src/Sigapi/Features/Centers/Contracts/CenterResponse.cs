namespace Sigapi.Features.Centers.Contracts;

[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed record CenterResponse
{
    public string Id { get; init; } = string.Empty;

    public string Slug { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string Acronym { get; init; } = string.Empty;

    public string? Address { get; init; }

    public string? Director { get; init; }

    public string? Description { get; init; }

    public string? LogoUrl { get; init; }

    public IEnumerable<DepartmentResponse> Departments { get; init; } = [];

    public IEnumerable<ProgramResponse> Programs { get; init; } = [];
    
    public IEnumerable<ResearchResponse> Researches { get; init; } = [];
}