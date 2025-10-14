using System.Text.Json.Serialization;

namespace Sigapi.Features.Centers.Contracts;

/// <summary>
/// Representa um curso de graduação ou pós-graduação.
/// </summary>
[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed record ProgramResponse
{
    /// <summary>
    /// O identificador único do curso.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// O nome do curso.
    /// </summary>
    [JsonPropertyName("nome")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// O nível do curso (graduação ou pós-graduação).
    /// </summary>
    [JsonPropertyName("tipo")]
    public ProgramType Type { get; init; }

    /// <summary>
    /// A cidade onde o curso é ofertado. Pode ser nulo.
    /// </summary>
    [JsonPropertyName("cidade")]
    public string? City { get; init; }
        
    /// <summary>
    /// A modalidade do curso (presencial ou a distância). Pode ser nulo.
    /// </summary>
    [JsonPropertyName("modalidade")]
    public ProgramModality? Modality { get; init; }

    /// <summary>
    /// O nome do coordenador(a) do curso. Pode ser nulo.
    /// </summary>
    [JsonPropertyName("coordenador")]
    public string? Coordinator { get; init; }
}