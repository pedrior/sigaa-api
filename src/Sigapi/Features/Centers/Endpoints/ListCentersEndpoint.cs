using Sigapi.Common.Cache;
using Sigapi.Common.Endpoints;
using Sigapi.Features.Centers.Contracts;
using Sigapi.Features.Centers.Models;
using Sigapi.Features.Centers.Scraping;
using Sigapi.Scraping.Browsing;
using Sigapi.Scraping.Engine;

namespace Sigapi.Features.Centers.Endpoints;

[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed class ListCentersEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder route)
    {
        route.MapGet("/", HandleAsync)
            .CacheOutput(CachePolicies.Centers.ListCenters)
            .Produces<IEnumerable<CenterResponse>>();
    }

    /// <summary>
    /// Lista todos os centros acadêmicos.
    /// </summary>
    /// <remarks>
    /// Retorna uma lista com informações básicas de todos os centros acadêmicos da instituição,
    /// como ID, nome, sigla e slug.
    /// </remarks>
    /// <returns>Uma lista de centros acadêmicos.</returns>
    /// <response code="200">Retorna a lista de centros acadêmicos.</response>
    internal static async Task<IResult> HandleAsync(HttpContext context,
        IResourceLoader resourceLoader,
        IScrapingEngine scrapingEngine,
        CancellationToken cancellationToken)
    {
        var document = await resourceLoader.LoadDocumentAsync(CenterPages.CenterList)
            .WithAnonymousSession(cancellationToken);

        var centers = await scrapingEngine.ScrapeAllAsync<Center>(document, cancellationToken);
        var response = centers
            .OrderBy(f => f.Name)
            .Select(f => new CenterResponse
            {
                Id = f.Id,
                Slug = f.Slug,
                Name = f.Name,
                Acronym = f.Acronym
            });

        return Results.Ok(response);
    }
}