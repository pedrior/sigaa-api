namespace Sigapi.Features.Account.Contracts;

/// <summary>
/// Representa o corpo da requisição para autenticação de um estudante.
/// </summary>
[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.Members)]
internal sealed record LoginRequest
{
    /// <summary>
    /// O nome de usuário (login) do estudante.
    /// </summary>
    public string Username { get; init; } = null!;

    /// <summary>
    /// A senha de acesso do estudante.
    /// </summary>
    public string Password { get; init; } = null!;

    /// <summary>
    /// A matrícula que referencia um vínculo/curso específico do estudante.
    /// </summary>
    /// <remarks>
    /// Este campo é opcional. Se não for especificado e o estudante possuir mais de uma matrícula,
    /// o sistema utilizará a mais recente por padrão.
    /// </remarks>
    /// <example>20190112416</example>
    public string? Enrollment { get; init; }
}