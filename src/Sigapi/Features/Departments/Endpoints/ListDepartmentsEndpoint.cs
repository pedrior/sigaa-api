using Sigapi.Common.Cache;
using Sigapi.Common.Endpoints;
using Sigapi.Features.Departments.Contracts;
using Sigapi.Features.Departments.Models;
using Sigapi.Features.Departments.Scraping;
using Sigapi.Scraping.Engine;
using Sigapi.Scraping.Networking;
using Sigapi.Scraping.Networking.Sessions;

namespace Sigapi.Features.Departments.Endpoints;

[UsedImplicitly]
internal sealed class ListDepartmentsEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/", HandleAsync)
            .CacheOutput(CachePolicies.Departments.ListDepartments)
            .WithSummary("Listar todas as unidades acadêmicas")
            .WithDescription("Retorna uma lista com todas as unidades acadêmicas agrupadas por seus respectivos " +
                             "centros acadêmicos.")
            .Produces<IEnumerable<DepartmentGroupResponse>>();
    }

    private static async Task<IResult> HandleAsync(HttpContext context,
        IPageFetcher pageFetcher,
        IScrapingEngine scrapingEngine,
        ISessionManager sessionManager,
        CancellationToken cancellationToken)
    {
        var session = sessionManager.CreateSession();
        var form = await GetDepartmentListingFormAsync(pageFetcher, scrapingEngine, session, cancellationToken);
        var page = await pageFetcher.FetchAndParseWithFormSubmissionAsync(
            form.Action,
            form.BuildSubmissionData(),
            session,
            cancellationToken);

        var departments = await scrapingEngine.ScrapeAllAsync<Department>(page, cancellationToken);
        var centers = await scrapingEngine.ScrapeAllAsync<DepartmentCenter>(page, cancellationToken);

        // Create a response with the departments grouped by their centers.
        var response = centers.GroupJoin(
            inner: departments,
            outerKeySelector: center => center.Acronym,
            innerKeySelector: department => department.CenterAcronym,
            resultSelector: (center, department) => new DepartmentGroupResponse
            {
                CenterName = center.Name,
                CenterSlug = center.Slug,
                Departments = department.Select(d => new DepartmentResponse
                {
                    Id = d.Id,
                    Name = d.Name,
                }).OrderBy(d => d.Name)
            }).OrderBy(g => g.CenterName);

        return Results.Ok(response);
    }

    private static async Task<DepartmentListingForm> GetDepartmentListingFormAsync(IPageFetcher pageFetcher,
        IScrapingEngine scrapingEngine,
        Sigapi.Scraping.Networking.Sessions.ISession session,
        CancellationToken cancellationToken)
    {
        var page = await pageFetcher.FetchAndParseAsync(
            DepartmentPages.Listing,
            session,
            cancellationToken);

        return scrapingEngine.Scrape<DepartmentListingForm>(page);
    }
}