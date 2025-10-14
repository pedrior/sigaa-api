using System.Text.Json.Serialization;

namespace Sigaa.Api.Features.Account.Contracts;

/// <summary>
/// Representa os dados do perfil de um estudante.
/// </summary>
[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed record ProfileResponse
{
    /// <summary>
    /// O nome completo do estudante.
    /// </summary>
    [JsonPropertyName("nome")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// O endereço de e-mail principal do estudante.
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// O nome de usuário (login) utilizado na autenticação.
    /// </summary>
    [JsonPropertyName("login")]
    public string Username { get; init; } = string.Empty;

    /// <summary>
    /// A matrícula do vínculo (curso) atualmente ativa na sessão.
    /// </summary>
    [JsonPropertyName("matricula")]
    public string Enrollment { get; init; } = string.Empty;

    /// <summary>
    /// O tipo de programa ao qual o estudante está vinculado.
    /// </summary>
    [JsonPropertyName("programa_tipo")]
    public ProgramType ProgramType { get; init; }

    /// <summary>
    /// Uma lista com todas as matrículas (ativas e inativas) associadas ao estudante.
    /// </summary>
    [JsonPropertyName("matriculas")]
    public IEnumerable<string> Enrollments { get; init; } = [];

    /// <summary>
    /// A URL para a foto de perfil do estudante. Pode ser nulo se não houver foto.
    /// </summary>
    [JsonPropertyName("foto")]
    public string? Photo { get; init; }

    /// <summary>
    /// A biografia ou descrição pessoal do estudante. Pode ser nulo.
    /// </summary>
    [JsonPropertyName("biografia")]
    public string? Biography { get; init; }

    /// <summary>
    /// As áreas de interesse ou pesquisa do estudante. Pode ser nulo.
    /// </summary>
    [JsonPropertyName("interesses")]
    public string? Interests { get; init; }

    /// <summary>
    /// A URL para o currículo Lattes do estudante. Pode ser nulo.
    /// </summary>
    [JsonPropertyName("curriculo")]
    public string? Curriculum { get; init; }
}