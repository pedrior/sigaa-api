using Sigaa.Api.Common.Caching;
using Sigaa.Api.Common.Endpoints;
using Sigaa.Api.Common.Scraping;
using Sigaa.Api.Common.Scraping.Client;
using Sigaa.Api.Features.Departments.Contracts;
using Sigaa.Api.Features.Departments.Models;
using Sigaa.Api.Features.Departments.Scraping;

namespace Sigaa.Api.Features.Departments.Endpoints;

[UsedImplicitly]
internal sealed class ListDepartmentsEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder route)
    {
        route.MapGet("/", HandleAsync)
            .CacheOutput(CachePolicies.Departments.ListDepartments)
            .Produces<IEnumerable<DepartmentResponse>>();
    }

    /// <summary>
    /// Lista todos os departamentos.
    /// </summary>
    /// <remarks>
    /// Retorna uma lista com informações de todos os departamentos da instituição, incluindo o centro acadêmico
    /// ao qual pertencem.
    /// </remarks>
    /// <returns>Uma lista de departamentos.</returns>
    /// <response code="200">Retorna a lista de departamentos.</response>
    internal static async Task<IResult> HandleAsync(HttpContext context,
        IFetcher fetcher,
        IScraper scraper,
        CancellationToken cancellationToken)
    {
        var form = await GetDepartmentListingFormAsync(fetcher, scraper, cancellationToken);
        var document = await fetcher.FetchDocumentAsync(form.Action, cancellationToken)
            .WithFormData(form.BuildSubmissionData())
            .WithEphemeralSession();

        var centers = await scraper.ScrapeAllAsync<DepartmentCenter>(document, cancellationToken);
        var departments = await scraper.ScrapeAllAsync<Department>(document, cancellationToken);

        var response = departments.Join(
                inner: centers,
                outerKeySelector: d => d.CenterAcronym,
                innerKeySelector: c => c.Acronym,
                resultSelector: (d, c) => new DepartmentResponse
                {
                    Id = d.Id,
                    Name = d.Name,
                    CenterSlug = c.Slug,
                    CenterName = c.Name
                })
            .OrderBy(r => r.CenterName)
            .ThenBy(r => r.Name);

        return Results.Ok(response);
    }

    private static async Task<DepartmentListingForm> GetDepartmentListingFormAsync(IFetcher fetcher,
        IScraper scraper,
        CancellationToken cancellationToken)
    {
        var document = await fetcher.FetchDocumentAsync(DepartmentPages.Listing, cancellationToken)
            .WithEphemeralSession();

        return scraper.Scrape<DepartmentListingForm>(document);
    }
}