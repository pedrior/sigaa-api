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
            .WithSummary("Logout")
            .WithDescription("Desconecta o estudante autenticado, invalidando sua sessão e token de acesso.")
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> HandleAsync(HttpContext context,
        IResourceLoader resourceLoader,
        IUserContext userContext,
        ISessionManager sessionManager,
        CancellationToken cancellationToken)
    {
        _ = await resourceLoader.LoadDocumentAsync(AccountPages.Logout)
            .WithUserSession(cancellationToken);

        await sessionManager.RevokeSessionAsync(userContext.SessionId, cancellationToken);

        return Results.NoContent();
    }
}