namespace Sigapi.Features.Centers.Models;

[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed class Research
{
    public string Name { get; set; } = string.Empty;
    
    public ResearchCoordinator Coordinator { get; set; } = new();
}