using Sigaa.Api.Common.Caching;
using Sigaa.Api.Common.Endpoints;
using Sigaa.Api.Common.Scraping;
using Sigaa.Api.Common.Scraping.Browsing;
using Sigaa.Api.Features.Departments.Contracts;
using Sigaa.Api.Features.Departments.Models;
using Sigaa.Api.Features.Departments.Scraping;

namespace Sigaa.Api.Features.Departments.Endpoints;

[UsedImplicitly]
internal sealed class ListDepartmentsEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/", HandleAsync)
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
        IResourceLoader resourceLoader,
        IScrapingEngine scrapingEngine,
        CancellationToken cancellationToken)
    {
        var form = await GetDepartmentListingFormAsync(resourceLoader, scrapingEngine, cancellationToken);
        var document = await resourceLoader.LoadDocumentAsync(form.Action)
            .WithFormData(form.BuildSubmissionData())
            .WithContextualSession(cancellationToken);

        var centers = await scrapingEngine.ScrapeAllAsync<DepartmentCenter>(document, cancellationToken);
        var departments = await scrapingEngine.ScrapeAllAsync<Department>(document, cancellationToken);

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

    private static async Task<DepartmentListingForm> GetDepartmentListingFormAsync(IResourceLoader resourceLoader,
        IScrapingEngine scrapingEngine,
        CancellationToken cancellationToken)
    {
        var document = await resourceLoader.LoadDocumentAsync(DepartmentPages.Listing)
            .WithContextualSession(cancellationToken);

        return scrapingEngine.Scrape<DepartmentListingForm>(document);
    }
}