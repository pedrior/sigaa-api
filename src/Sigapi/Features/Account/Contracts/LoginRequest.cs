namespace Sigapi.Features.Account.Contracts;

[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.Members)]
internal sealed record LoginRequest
{
    public string Username { get; init; } = null!;

    public string Password { get; init; } = null!;

    public string? Enrollment { get; init; }
}