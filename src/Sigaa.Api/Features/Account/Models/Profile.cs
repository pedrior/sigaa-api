namespace Sigaa.Api.Features.Account.Models;

[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.Members)]
internal sealed class Profile
{
    public string Name { get; set; } = string.Empty;
   
    public string Email { get; set; } = string.Empty;

    public string Enrollment { get; set; } = string.Empty;

    public bool IsProgramCompletionAvailable { get; set; }

    public string? Photo { get; set; }

    public string? Biography { get; set; }

    public string? Interests { get; set; }

    public string? Curriculum { get; set; }
}