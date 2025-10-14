using System.Text.Json.Serialization;

namespace Sigapi.Features.Centers.Contracts;

/// <summary>
/// Representa um projeto de pesquisa.
/// </summary>
[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed record ResearchResponse
{
    /// <summary>
    /// O nome do projeto de pesquisa.
    /// </summary>
    [JsonPropertyName("nome")]
    public string Name { get; init; } = string.Empty;
    
    /// <summary>
    /// O professor coordenador do projeto de pesquisa.
    /// </summary>
    [JsonPropertyName("coordenador")]
    public ResearchCoordinatorResponse Coordinator { get; init; } = new();
}
