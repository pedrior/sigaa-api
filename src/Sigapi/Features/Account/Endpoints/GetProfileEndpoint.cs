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
            .Produces<ProfileResponse>();
    }

    /// <summary>
    /// Obtém o perfil do estudante autenticado.
    /// </summary>
    /// <remarks>
    /// Retorna informações detalhadas do perfil do estudante associado ao token de autenticação,
    /// como nome, e-mail, matrícula, foto e outros dados pessoais.
    /// </remarks>
    /// <returns>O perfil detalhado do estudante.</returns>
    /// <response code="200">Retorna as informações do perfil do estudante.</response>
    /// <response code="401">Usuário não autenticado.</response>
    internal static async Task<IResult> HandleAsync(HttpContext context,
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
            ProgramType = profile.IsProgramCompletionAvailable
                ? ProgramType.Undergraduate
                : ProgramType.Postgraduate,
            Enrollments = userContext.Enrollments,
            Photo = profile.Photo,
            Biography = profile.Biography,
            Interests = profile.Interests,
            Curriculum = profile.Curriculum,
        };

        return Results.Ok(response);
    }
}