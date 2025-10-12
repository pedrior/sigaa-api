namespace Sigapi.Features.Centers.Contracts;

/// <summary>
/// Representa o professor coordenador de um projeto de pesquisa.
/// </summary>
[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed record ResearchCoordinatorResponse
{
    /// <summary>
    /// O identificador SIAPE do professor coordenador.
    /// </summary>
    public string Id { get; init; } = string.Empty;
    
    /// <summary>
    /// O nome do professor coordenador.
    /// </summary>
    public string Name { get; init; } = string.Empty;
}
