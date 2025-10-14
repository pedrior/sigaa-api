using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sigaa.Api.Features.Account.Contracts;

/// <summary>
/// Representa o corpo da requisição para autenticação de um estudante.
/// </summary>
[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.Members)]
public sealed record LoginRequest
{
    /// <summary>
    /// O nome de usuário (login) do estudante.
    /// </summary>
    [JsonPropertyName("login")]
    [Required(ErrorMessage = "Must not be null or empty."), MinLength(6)]
    public string Username { get; init; } = null!;

    /// <summary>
    /// A senha de acesso do estudante.
    /// </summary>
    [JsonPropertyName("senha")]
    [Required(ErrorMessage = "Must not be null or empty.")]
    public string Password { get; init; } = null!;

    /// <summary>
    /// A matrícula que referencia um vínculo/curso específico do estudante.
    /// </summary>
    /// <remarks>
    /// Este campo é opcional. Se não for especificado e o estudante possuir mais de uma matrícula,
    /// o sistema utilizará a mais recente por padrão.
    /// </remarks>
    [JsonPropertyName("matricula")]
    [RegularExpression("^[0-9]*$", ErrorMessage = "Must be a valid enrollment identifier consisting of digits only.")]
    public string? Enrollment { get; init; }
}