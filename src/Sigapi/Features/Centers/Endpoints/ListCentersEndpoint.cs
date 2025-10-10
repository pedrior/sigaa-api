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
            .WithSummary("Listar todos os centros acadêmico")
            .WithDescription("Retorna uma lista com todas os centros acadêmico disponíveis.")
            .Produces<IEnumerable<CenterResponse>>();
    }

    private static async Task<IResult> HandleAsync(HttpContext context,
        IResourceLoader resourceLoader,
        IScrapingEngine scrapingEngine,
        CancellationToken cancellationToken)
    {
        var response = await ListCentersAsync(resourceLoader, scrapingEngine, cancellationToken);

        return Results.Ok(response);
    }

    private static async Task<IEnumerable<CenterResponse>> ListCentersAsync(IResourceLoader resourceLoader,
        IScrapingEngine scrapingEngine,
        CancellationToken cancellationToken)
    {
        var page = await resourceLoader.LoadDocumentAsync(CenterPages.CenterList)
            .WithAnonymousSession(cancellationToken);

        var centers = await scrapingEngine.ScrapeAllAsync<Center>(page, cancellationToken);
        return centers
            .OrderBy(f => f.Name)
            .Select(f => new CenterResponse
            {
                Id = f.Id,
                Slug = f.Slug,
                Name = f.Name,
                Acronym = f.Acronym
            });
    }
}