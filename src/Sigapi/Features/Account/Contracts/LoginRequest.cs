using System.ComponentModel.DataAnnotations;

namespace Sigapi.Features.Account.Contracts;

/// <summary>
/// Representa o corpo da requisição para autenticação de um estudante.
/// </summary>
[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.Members)]
public sealed record LoginRequest
{
    /// <summary>
    /// O nome de usuário (login) do estudante.
    /// </summary>
    [Required(ErrorMessage = "Must not be null or empty."), MinLength(6)]
    public string Username { get; init; } = null!;

    /// <summary>
    /// A senha de acesso do estudante.
    /// </summary>
    [Required(ErrorMessage = "Must not be null or empty.")]
    public string Password { get; init; } = null!;

    /// <summary>
    /// A matrícula que referencia um vínculo/curso específico do estudante.
    /// </summary>
    /// <remarks>
    /// Este campo é opcional. Se não for especificado e o estudante possuir mais de uma matrícula,
    /// o sistema utilizará a mais recente por padrão.
    /// </remarks>
    /// <example>20190112416</example>
    [RegularExpression("^[0-9]*$", ErrorMessage = "Must be a valid enrollment identifier consisting of digits only.")]
    public string? Enrollment { get; init; }
}