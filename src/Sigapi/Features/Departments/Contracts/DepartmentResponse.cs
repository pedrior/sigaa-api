using System.Text.Json.Serialization;

namespace Sigapi.Features.Departments.Contracts;

/// <summary>
/// Representa os dados de um departamento.
/// </summary>
[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed record DepartmentResponse
{
    /// <summary>
    /// O identificador único do departamento.
    /// </summary>
    public string Id { get; init; } = null!;
    
    /// <summary>
    /// O nome do departamento.
    /// </summary>
    [JsonPropertyName("nome")]
    public string Name { get; init; } = null!;
    
    /// <summary>
    /// O <c>slug</c> (nome para URL) do centro acadêmico ao qual o departamento pertence.
    /// </summary>
    [JsonPropertyName("centro_slug")]
    public string CenterSlug { get; init; } = null!;
    
    /// <summary>
    /// O nome do centro acadêmico ao qual o departamento pertence.
    /// </summary>
    [JsonPropertyName("centro_nome")]
    public string CenterName { get; init; } = null!;
}
