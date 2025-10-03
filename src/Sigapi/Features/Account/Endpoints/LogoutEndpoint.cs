using Sigapi.Common.Endpoints;
using Sigapi.Common.RateLimiter;
using Sigapi.Common.Security;
using Sigapi.Features.Account.Scraping;
using Sigapi.Scraping.Networking;
using Sigapi.Scraping.Networking.Sessions;

namespace Sigapi.Features.Account.Endpoints;

[UsedImplicitly]
internal sealed class LogoutEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder route)
    {
        route.MapPost("/logout", HandleAsync)
            .RequireRateLimiting(RateLimiterPolicies.Account.SessionManagement)
            .WithSummary("Logout")
            .WithDescription("Desconecta o estudante autenticado, invalidando sua sessão e token de acesso.")
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> HandleAsync(HttpContext context,
        IPageFetcher pageFetcher,
        IUserContext userContext,
        ISessionManager sessionManager,
        CancellationToken cancellationToken)
    {
        var session = await sessionManager.LoadSessionAsync(userContext.SessionId, cancellationToken);

        await pageFetcher.FetchAndParseAsync(AccountPages.Logout, session, cancellationToken);
        await sessionManager.RevokeSessionAsync(userContext.SessionId, cancellationToken);

        return Results.NoContent();
    }
}