namespace Sigapi.Features.Centers.Contracts;

[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed record CenterSummaryResponse
{
    public string Id { get; init; } = null!;

    public string Slug { get; init; } = null!;

    public string Name { get; init; } = null!;

    public string Acronym { get; init; } = null!;
}