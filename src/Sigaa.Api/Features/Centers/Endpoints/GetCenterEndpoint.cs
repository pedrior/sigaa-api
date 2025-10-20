using Sigaa.Api.Common.Caching;
using Sigaa.Api.Common.Endpoints;
using Sigaa.Api.Common.Problems;
using Sigaa.Api.Common.Scraping;
using Sigaa.Api.Common.Scraping.Client;
using Sigaa.Api.Features.Centers.Contracts;
using Sigaa.Api.Features.Centers.Models;
using Sigaa.Api.Features.Centers.Scraping;

namespace Sigaa.Api.Features.Centers.Endpoints;

[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed class GetCenterEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder route)
    {
        route.MapGet("/{idOrSlug}", HandleAsync)
            .CacheOutput(CachePolicies.Centers.GetCenter)
            .Produces<CenterDetailsResponse>();
    }

    /// <summary>
    /// Obtém detalhes de um centro acadêmico específico.
    /// </summary>
    /// <remarks>
    /// Retorna informações detalhadas de um centro acadêmico, incluindo departamentos, cursos e projetos de
    /// pesquisa associados. A busca pode ser feita tanto pelo ID numérico quanto pelo <c>slug</c> (nome amigável para URL).
    /// </remarks>
    /// <param name="idOrSlug" example="centro-de-informatica">O ID ou o slug do centro acadêmico a ser consultado.</param>
    /// <returns>Os detalhes do centro acadêmico.</returns>
    /// <response code="200">Retorna os detalhes do centro acadêmico solicitado.</response>
    /// <response code="404">Se o centro acadêmico com o ID ou slug especificado não for encontrado.</response>
    internal static async Task<IResult> HandleAsync(string idOrSlug,
        HttpContext context,
        IFetcher fetcher,
        IScraper scraper,
        CancellationToken cancellationToken)
    {
        if (await FindCenterAsync(idOrSlug, fetcher, scraper, cancellationToken) is not { } center)
        {
            return idOrSlug.All(char.IsDigit)
                ? new NotFoundProblem($"Center with ID '{idOrSlug}' was not found.")
                : new NotFoundProblem($"Center with slug '{idOrSlug}' was not found.");
        }

        var response = await GetCenterDetailsAsync(
            center,
            fetcher,
            scraper,
            cancellationToken);

        return Results.Ok(response);
    }

    private static async Task<Center?> FindCenterAsync(string idOrSlug,
        IFetcher fetcher,
        IScraper scraper,
        CancellationToken cancellationToken)
    {
        var document = await fetcher.FetchDocumentAsync(CenterPages.CenterList, cancellationToken);
        var centers = await scraper.ScrapeAllAsync<Center>(document, cancellationToken);

        return centers.FirstOrDefault(c => c.Id == idOrSlug || c.Slug == idOrSlug);
    }

    private static async Task<CenterDetailsResponse> GetCenterDetailsAsync(Center center,
        IFetcher fetcher,
        IScraper scraper,
        CancellationToken cancellationToken)
    {
        var centerDocumentTask = fetcher.FetchDocumentAsync(
            CenterPages.GetCenter(center.Id),
            cancellationToken).AsTask();

        var departmentsDocumentTask = fetcher.FetchDocumentAsync(
            CenterPages.GetDepartments(center.Id),
            cancellationToken).AsTask();

        var undergraduateProgramsDocumentTask = fetcher.FetchDocumentAsync(
            CenterPages.GetUndergraduatePrograms(center.Id),
            cancellationToken).AsTask();

        var postgraduateProgramsDocumentTask = fetcher.FetchDocumentAsync(
            CenterPages.GetPostgraduatePrograms(center.Id),
            cancellationToken).AsTask();

        var researchesDocumentTask = fetcher.FetchDocumentAsync(
            CenterPages.GetResearches(center.Id),
            cancellationToken).AsTask();

        await Task.WhenAll(
            centerDocumentTask,
            departmentsDocumentTask,
            undergraduateProgramsDocumentTask,
            postgraduateProgramsDocumentTask,
            researchesDocumentTask);

        var centerDocument = centerDocumentTask.Result;
        var departmentsDocument = departmentsDocumentTask.Result;
        var undergraduateProgramsDocument = undergraduateProgramsDocumentTask.Result;
        var postgraduateProgramsDocument = postgraduateProgramsDocumentTask.Result;
        var researchesDocument = researchesDocumentTask.Result;

        var details = scraper.Scrape<CenterDetails>(centerDocument);

        var departmentsTask = scraper.ScrapeAllAsync<Department>(departmentsDocument, cancellationToken);

        var undergraduateProgramsTask = scraper.ScrapeAllAsync<UndergraduateProgram>(
            undergraduateProgramsDocument,
            cancellationToken);

        var postgraduateProgramsTask = scraper.ScrapeAllAsync<GraduateProgram>(
            postgraduateProgramsDocument,
            cancellationToken);

        var researchesTask = scraper.ScrapeAllAsync<Research>(researchesDocument, cancellationToken);

        await Task.WhenAll(departmentsTask, undergraduateProgramsTask, postgraduateProgramsTask, researchesTask);

        var departmentsResponse = departmentsTask.Result
            .OrderBy(d => d.Name)
            .Select(d => new DepartmentResponse
            {
                Id = d.Id,
                Name = d.Name
            });

        var programsResponse = undergraduateProgramsTask.Result
            .OrderBy(p => p.Name)
            .Select(p => new ProgramResponse
            {
                Id = p.Id,
                Name = p.Name,
                Type = ProgramType.Undergraduate,
                Modality = p.IsOnsite
                    ? ProgramModality.InPerson
                    : ProgramModality.Distance,
                Coordinator = p.Coordinator,
                City = p.City,
            })
            // Merge with postgraduate programs
            .Concat(postgraduateProgramsTask.Result
                .OrderBy(p => p.Name)
                .Select(p => new ProgramResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Type = ProgramType.Postgraduate
                }));

        var researchResponses = researchesTask.Result
            .OrderBy(r => r.Name)
            .Select(r => new ResearchResponse
            {
                Name = r.Name,
                Coordinator = new ResearchCoordinatorResponse
                {
                    Id = r.Coordinator.Id,
                    Name = r.Coordinator.Name
                }
            });

        return new CenterDetailsResponse
        {
            Id = center.Id,
            Slug = center.Slug,
            Name = center.Name,
            Acronym = center.Acronym,
            Address = details.Address,
            Director = details.Director,
            Description = details.Description,
            LogoUrl = details.LogoUrl,
            Departments = departmentsResponse,
            Programs = programsResponse,
            Researches = researchResponses
        };
    }
}