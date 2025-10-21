using System.Text.Json.Serialization;

namespace Sigaa.Api.Features.Departments.Contracts;

/// <summary>
/// Representa os dados detalhados de um departamento acadêmico.
/// </summary>
[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed record DepartmentDetailsResponse
{
    /// <summary>
    /// O código único do departamento.
    /// </summary>
    [JsonPropertyName("codigo")] 
    public string Code { get; init; } = null!;

    /// <summary>
    /// O nome completo do departamento.
    /// </summary>
    [JsonPropertyName("nome")] 
    public string Name { get; init; } = null!;

    /// <summary>
    /// A sigla do departamento.
    /// </summary>
    [JsonPropertyName("sigla")] 
    public string Acronym { get; init; } = null!;
    
    /// <summary>
    /// A URL para o logotipo do departamento.
    /// </summary>
    [JsonPropertyName("logo_url")] 
    public string LogoUrl { get; init; } = null!;
    
    /// <summary>
    /// O nome do chefe do departamento. Pode ser nulo se não informado.
    /// </summary>
    [JsonPropertyName("chefia_nome")]
    public string? HeadName { get; init; }
    
    /// <summary>
    /// O código SIAPE do chefe do departamento. Pode ser nulo se não informado.
    /// </summary>
    [JsonPropertyName("chefia_siape")]
    public string? HeadSiape { get; init; }
    
    /// <summary>
    /// O texto de apresentação do departamento. Pode ser nulo se não informado.
    /// </summary>
    [JsonPropertyName("apresentacao")] 
    public string? Presentation { get; init; }

    /// <summary>
    /// O endereço alternativo do departamento. Pode ser nulo se não informado.
    /// </summary>
    [JsonPropertyName("endereco_alternativo")]
    public string? AlternativeAddress { get; init; }
}
