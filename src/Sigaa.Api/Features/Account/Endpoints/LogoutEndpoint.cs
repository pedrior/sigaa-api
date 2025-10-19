using Sigaa.Api.Common.Endpoints;
using Sigaa.Api.Common.RateLimiting;
using Sigaa.Api.Common.Scraping.Client;
using Sigaa.Api.Common.Scraping.Client.Sessions;
using Sigaa.Api.Features.Account.Scraping;

namespace Sigaa.Api.Features.Account.Endpoints;

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
        IFetcher fetcher,
        ISessionRevoker sessionRevoker,
        CancellationToken cancellationToken)
    {
        await fetcher.FetchDocumentAsync(AccountPages.Logout, cancellationToken)
            .WithPersistentSession();

        await sessionRevoker.RevokeAsync(cancellationToken);

        return Results.NoContent();
    }
}