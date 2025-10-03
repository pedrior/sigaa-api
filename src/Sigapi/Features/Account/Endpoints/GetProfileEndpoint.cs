using Sigapi.Common.Cache;
using Sigapi.Common.Endpoints;
using Sigapi.Common.RateLimiter;
using Sigapi.Common.Security;
using Sigapi.Features.Account.Contracts;
using Sigapi.Features.Account.Models;
using Sigapi.Features.Account.Scraping;
using Sigapi.Scraping.Engine;
using Sigapi.Scraping.Networking;
using Sigapi.Scraping.Networking.Sessions;

namespace Sigapi.Features.Account.Endpoints;

[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed class GetProfileEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder route)
    {
        route.MapGet("/profile", HandleAsync)
            .CacheOutput(CachePolicies.Account.GetProfile)
            .RequireRateLimiting(RateLimiterPolicies.Authenticated)
            .WithSummary("Obter Perfil")
            .WithDescription("Retorna as informações de perfil do estudante autenticado.")
            .Produces<ProfileResponse>();
    }

    private static async Task<IResult> HandleAsync(HttpContext context,
        IPageFetcher pageFetcher,
        IScrapingEngine scrapingEngine,
        IUserContext userContext,
        ISessionManager sessionManager,
        CancellationToken cancellationToken)
    {
        var response = await GetProfileAsync(
            pageFetcher,
            scrapingEngine,
            userContext,
            sessionManager,
            cancellationToken);

        return Results.Ok(response);
    }

    private static async Task<ProfileResponse> GetProfileAsync(IPageFetcher pageFetcher,
        IScrapingEngine scrapingEngine,
        IUserContext userContext,
        ISessionManager sessionManager,
        CancellationToken cancellationToken = default)
    {
        var session = await sessionManager.LoadSessionAsync(userContext.SessionId, cancellationToken);
        var page = await pageFetcher.FetchAndParseAsync(AccountPages.Profile, session, cancellationToken);

        var profile = scrapingEngine.Scrape<Profile>(page);

        return new ProfileResponse
        {
            Name = profile.Name,
            Email = profile.Email,
            Username = userContext.Username,
            Enrollment = userContext.Enrollment,
            EnrollmentType = profile.IsProgramCompletionAvailable
                ? EnrollmentType.Undergraduate
                : EnrollmentType.Postgraduate,
            Enrollments = userContext.Enrollments,
            Photo = profile.Photo,
            Biography = profile.Biography,
            Interests = profile.Interests,
            Curriculum = profile.Curriculum,
        };
    }
}