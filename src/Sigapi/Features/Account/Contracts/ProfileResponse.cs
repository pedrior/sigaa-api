namespace Sigapi.Features.Account.Contracts;

[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed record ProfileResponse
{
    public string Name { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Username { get; init; } = string.Empty;

    public string Enrollment { get; init; } = string.Empty;

    public EnrollmentType EnrollmentType { get; init; }

    public IEnumerable<string> Enrollments { get; init; } = [];

    public string? Photo { get; init; }

    public string? Biography { get; init; }

    public string? Interests { get; init; }

    public string? Curriculum { get; init; }
}