using System.Text.Json.Serialization;

namespace Sigaa.Api.Features.Centers.Contracts;

/// <summary>
/// Representa os dados detalhados de um centro acadêmico.
/// </summary>
[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed record CenterDetailsResponse
{
    /// <summary>
    /// O identificador único do centro.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Uma versão do nome do centro otimizada para URLs.
    /// </summary>
    public string Slug { get; init; } = string.Empty;

    /// <summary>
    /// O nome completo do centro acadêmico.
    /// </summary>
    [JsonPropertyName("nome")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// A sigla do centro acadêmico.
    /// </summary>
    [JsonPropertyName("sigla")]
    public string Acronym { get; init; } = string.Empty;

    /// <summary>
    /// O endereço alternativo do centro. Pode ser nulo se não informado.
    /// </summary>
    [JsonPropertyName("endereco")]
    public string? Address { get; init; }

    /// <summary>
    /// O nome do diretor(a) do centro. Pode ser nulo se não informado.
    /// </summary>
    [JsonPropertyName("diretor")]
    public string? Director { get; init; }

    /// <summary>
    /// Texto de apresentação do centro. Pode ser nulo se não informado.
    /// </summary>
    [JsonPropertyName("descricao")]
    public string? Description { get; init; }

    /// <summary>
    /// A URL para o logotipo do centro.
    /// </summary>
    [JsonPropertyName("logo_url")]
    public string? LogoUrl { get; init; }

    /// <summary>
    /// Lista de departamentos vinculados ao centro.
    /// </summary>
    [JsonPropertyName("departamentos")]
    public IEnumerable<DepartmentResponse> Departments { get; init; } = [];

    /// <summary>
    /// Lista de cursos (graduação e pós-graduação) vinculados ao centro.
    /// </summary>
    [JsonPropertyName("programas")]
    public IEnumerable<ProgramResponse> Programs { get; init; } = [];
    
    /// <summary>
    /// Lista de projetos de pesquisa vinculados ao centro.
    /// </summary>
    [JsonPropertyName("pesquisas")]
    public IEnumerable<ResearchResponse> Researches { get; init; } = [];
}