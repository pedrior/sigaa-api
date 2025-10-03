namespace Sigapi.Features.Account.Contracts;

[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.Members)]
internal sealed record LoginResponse
{
    public string Token { get; init; } = null!;

    public DateTimeOffset ExpiresAt { get; init; }
}