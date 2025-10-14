using System.Text.Json.Serialization;

namespace Sigaa.Api.Features.Centers.Contracts;

/// <summary>
/// Representa os dados básicos de um centro acadêmico.
/// </summary>
[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed record CenterResponse
{
    /// <summary>
    /// O identificador único do centro.
    /// </summary>
    public string Id { get; init; } = null!;

    /// <summary>
    /// Uma versão do nome do centro otimizada para URLs.
    /// </summary>
    public string Slug { get; init; } = null!;

    /// <summary>
    /// O nome completo do centro acadêmico.
    /// </summary>
    [JsonPropertyName("nome")]
    public string Name { get; init; } = null!;

    /// <summary>
    /// A sigla do centro acadêmico.
    /// </summary>
    [JsonPropertyName("sigla")]
    public string Acronym { get; init; } = null!;
}