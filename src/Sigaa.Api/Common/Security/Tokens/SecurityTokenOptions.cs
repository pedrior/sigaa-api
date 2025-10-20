using System.ComponentModel.DataAnnotations;
using Sigaa.Api.Common.Options;

namespace Sigaa.Api.Common.Security.Tokens;

internal sealed record SecurityTokenOptions : IOptions
{
    public static string SectionName => "Jwt";
    
    public string? Issuer { get; init; }

    public string? Audience { get; init; }

    [Required, MinLength(32)]
    public string Key { get; init; } = null!;
}