namespace Sigaa.Api.Features.Centers.Models;

[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed class CenterDetails
{
    public string Name { get; set; } = string.Empty;
    
    public string Acronym { get; set; } = string.Empty;

    public string? Address { get; set; }
    
    public string? Director { get; set; }
    
    public string? Description { get; set; }
  
    public string LogoUrl { get; set; } = null!;
}