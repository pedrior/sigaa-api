using Sigapi.Common.Endpoints;
using Sigapi.Common.RateLimiter;
using Sigapi.Common.Security;
using Sigapi.Features.Account.Scraping;
using Sigapi.Scraping.Browsing;
using Sigapi.Scraping.Browsing.Sessions;

namespace Sigapi.Features.Account.Endpoints;

[UsedImplicitly]
internal sealed class LogoutEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder route)
    {
        route.MapPost("/logout", HandleAsync)
            .RequireRateLimiting(RateLimiterPolicies.Account.SessionManagement)
            .Produces(StatusCodes.Status204NoContent);
    }

    /// <summary>
    /// Encerra a sessão do estudante autenticado (Logout).
    /// </summary>
    /// <remarks>
    /// Invalida o token de acesso atual, exigindo uma nova autenticação para futuras requisições.
    /// Este endpoint deve ser chamado quando o usuário desejar encerrar sua sessão na aplicação.
    /// </remarks>
    /// <returns>Nenhum conteúdo.</returns>
    /// <response code="204">Sessão encerrada com sucesso.</response>
    /// <response code="401">Usuário não autenticado.</response>
    internal static async Task<IResult> HandleAsync(HttpContext context,
        IResourceLoader resourceLoader,
        IUserContext userContext,
        ISessionManager sessionManager,
        CancellationToken cancellationToken)
    {
        await resourceLoader.LoadDocumentAsync(AccountPages.Logout)
            .WithUserSession(cancellationToken);

        await sessionManager.RevokeSessionAsync(userContext.SessionId, cancellationToken);

        return Results.NoContent();
    }
}