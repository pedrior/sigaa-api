using Sigaa.Api.Common.Caching;
using Sigaa.Api.Common.Endpoints;
using Sigaa.Api.Common.Problems;
using Sigaa.Api.Common.Scraping;
using Sigaa.Api.Common.Scraping.Client;
using Sigaa.Api.Features.Departments.Contracts;
using Sigaa.Api.Features.Departments.Models;
using Sigaa.Api.Features.Departments.Scraping;

namespace Sigaa.Api.Features.Departments.Endpoints;

[UsedImplicitly]
internal sealed class GetDepartmentEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder route)
    {
        route.MapGet("/{code}", HandleAsync)
            .CacheOutput(CachePolicies.Departments.GetDepartment)
            .Produces<DepartmentDetailsResponse>();
    }

    /// <summary>
    /// Obtém detalhes de um departamento específico pelo código.
    /// </summary>
    /// <remarks>
    /// Retorna informações detalhadas de um departamento acadêmico a partir do seu código único.
    /// </remarks>
    /// <returns>Os detalhes do departamento.</returns>
    /// <response code="200">Retorna os detalhes do departamento solicitado.</response>
    /// <response code="404">Se o departamento com o código especificado não for encontrado.</response>
    internal static async Task<IResult> HandleAsync(string code,
        IFetcher fetcher,
        IScraper scraper,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var form = await GetDepartmentListingFormAsync(fetcher, scraper, cancellationToken);
        var departmentListDocument = await fetcher.FetchDocumentAsync(form.Action, cancellationToken)
            .WithFormData(form.BuildSubmissionData())
            .WithEphemeralSession();
        
        var departmentEntries = await scraper.ScrapeAllAsync<DepartmentEntry>(
            departmentListDocument,
            cancellationToken);
        
        if (departmentEntries.All(d => d.Code != code))
        {
            return new NotFoundProblem($"Department with code '{code}' was not found.");
        }
        
        var departmentDocument = await fetcher.FetchDocumentAsync(
            DepartmentPages.GetDepartmentPageUrl(code),
            cancellationToken);

        var department = scraper.Scrape<DepartmentDetails>(departmentDocument);
        return Results.Ok(department.ToResponse());
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