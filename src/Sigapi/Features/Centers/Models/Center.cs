namespace Sigapi.Features.Centers.Models;

[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed class Center
{
    public string Id { get; set; } = string.Empty;
    
    public string Slug { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
    
    public string Acronym { get; set; } = string.Empty;
}