using Sigaa.Api.Common.Caching;
using Sigaa.Api.Common.Endpoints;
using Sigaa.Api.Common.Scraping;
using Sigaa.Api.Common.Scraping.Client;
using Sigaa.Api.Features.Centers.Contracts;
using Sigaa.Api.Features.Centers.Models;
using Sigaa.Api.Features.Centers.Scraping;

namespace Sigaa.Api.Features.Centers.Endpoints;

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
        IFetcher fetcher,
        IScraper scraper,
        CancellationToken cancellationToken)
    {
        var document = await fetcher.FetchDocumentAsync(CenterPages.CenterList, cancellationToken);
        var centers = await scraper.ScrapeAllAsync<Center>(document, cancellationToken);

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