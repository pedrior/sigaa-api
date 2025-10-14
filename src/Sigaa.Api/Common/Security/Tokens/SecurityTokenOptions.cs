using System.ComponentModel.DataAnnotations;

namespace Sigaa.Api.Common.Security.Tokens;

internal sealed record SecurityTokenOptions
{
    public string? Issuer { get; init; }

    public string? Audience { get; init; }

    [Required, MinLength(32)]
    public string Key { get; init; } = null!;
}