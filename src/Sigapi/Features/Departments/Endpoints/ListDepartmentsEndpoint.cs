using Sigapi.Common.Cache;
using Sigapi.Common.Endpoints;
using Sigapi.Features.Departments.Contracts;
using Sigapi.Features.Departments.Models;
using Sigapi.Features.Departments.Scraping;
using Sigapi.Scraping.Browsing;
using Sigapi.Scraping.Engine;

namespace Sigapi.Features.Departments.Endpoints;

[UsedImplicitly]
internal sealed class ListDepartmentsEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/", HandleAsync)
            .CacheOutput(CachePolicies.Departments.ListDepartments)
            .WithSummary("Listar todos os departamentos")
            .WithDescription("Obtém uma lista de todos os departamentos agrupados por seus respectivos " +
                             "centros acadêmicos.")
            .Produces<IEnumerable<DepartmentGroupResponse>>();
    }

    private static async Task<IResult> HandleAsync(HttpContext context,
        IResourceLoader resourceLoader,
        IScrapingEngine scrapingEngine,
        CancellationToken cancellationToken)
    {
        var form = await GetDepartmentListingFormAsync(resourceLoader, scrapingEngine, cancellationToken);
        var page = await resourceLoader.LoadDocumentAsync(form.Action)
            .WithFormData(form.BuildSubmissionData())
            .WithContextualSession(cancellationToken);

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

    private static async Task<DepartmentListingForm> GetDepartmentListingFormAsync(IResourceLoader resourceLoader,
        IScrapingEngine scrapingEngine,
        CancellationToken cancellationToken)
    {
        var page = await resourceLoader.LoadDocumentAsync(DepartmentPages.Listing)
            .WithContextualSession(cancellationToken);

        return scrapingEngine.Scrape<DepartmentListingForm>(page);
    }
}