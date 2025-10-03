namespace Sigapi.Features.Centers.Models;

[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed class UndergraduateProgram
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    
    public string City { get; set; } = string.Empty;

    public string? Coordinator { get; set; }
    
    public bool IsOnsite { get; set; }
}