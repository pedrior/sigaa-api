using Sigapi.Common.Cache;
using Sigapi.Common.Endpoints;
using Sigapi.Common.RateLimiter;
using Sigapi.Common.Security;
using Sigapi.Features.Account.Contracts;
using Sigapi.Features.Account.Models;
using Sigapi.Features.Account.Scraping;
using Sigapi.Scraping.Browsing;
using Sigapi.Scraping.Engine;

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
        IResourceLoader resourceLoader,
        IScrapingEngine scrapingEngine,
        IUserContext userContext,
        CancellationToken cancellationToken)
    {
        var document = await resourceLoader.LoadDocumentAsync(AccountPages.Profile)
            .WithUserSession(cancellationToken);

        var profile = scrapingEngine.Scrape<Profile>(document);
        var response = new ProfileResponse
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

        return Results.Ok(response);
    }
}