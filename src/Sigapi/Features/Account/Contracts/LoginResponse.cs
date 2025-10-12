namespace Sigapi.Features.Account.Contracts;

/// <summary>
/// Representa a resposta de uma autenticação bem-sucedida.
/// </summary>
[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.Members)]
internal sealed record LoginResponse
{
    /// <summary>
    /// O token de acesso JWT (JSON Web Token) gerado.
    /// </summary>
    /// <remarks>
    /// Este token deve ser enviado no cabeçalho <c>Authorization</c> de requisições subsequentes como
    /// <c>Bearer {token}</c>.
    /// </remarks>
    public string Token { get; init; } = null!;

    /// <summary>
    /// A data e hora (UTC) em que o token de acesso expira.
    /// </summary>
    public DateTimeOffset ExpiresAt { get; init; }
}
