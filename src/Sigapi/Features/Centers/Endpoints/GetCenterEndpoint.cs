using Sigapi.Common.Cache;
using Sigapi.Common.Endpoints;
using Sigapi.Common.Problems;
using Sigapi.Features.Centers.Contracts;
using Sigapi.Features.Centers.Models;
using Sigapi.Features.Centers.Scraping;
using Sigapi.Scraping.Browsing;
using Sigapi.Scraping.Engine;

namespace Sigapi.Features.Centers.Endpoints;

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
        IResourceLoader resourceLoader,
        IScrapingEngine scrapingEngine,
        CancellationToken cancellationToken)
    {
        if (await FindCenterAsync(idOrSlug, resourceLoader, scrapingEngine, cancellationToken) is not { } center)
        {
            return idOrSlug.All(char.IsDigit)
                ? new NotFoundProblem($"Center with ID '{idOrSlug}' was not found.")
                : new NotFoundProblem($"Center with slug '{idOrSlug}' was not found.");
        }

        var response = await GetCenterDetailsAsync(
            center,
            resourceLoader,
            scrapingEngine,
            cancellationToken);

        return Results.Ok(response);
    }

    private static async Task<Center?> FindCenterAsync(string idOrSlug,
        IResourceLoader resourceLoader,
        IScrapingEngine scrapingEngine,
        CancellationToken cancellationToken)
    {
        var document = await resourceLoader.LoadDocumentAsync(CenterPages.CenterList)
            .WithAnonymousSession(cancellationToken);

        var centers = await scrapingEngine.ScrapeAllAsync<Center>(document, cancellationToken);
        return centers.FirstOrDefault(c => c.Id == idOrSlug || c.Slug == idOrSlug);
    }

    private static async Task<CenterDetailsResponse> GetCenterDetailsAsync(Center center,
        IResourceLoader resourceLoader,
        IScrapingEngine scrapingEngine,
        CancellationToken cancellationToken)
    {
        var centerDocumentTask = resourceLoader.LoadDocumentAsync(CenterPages.GetCenter(center.Id))
            .WithAnonymousSession(cancellationToken)
            .AsTask();

        var departmentsDocumentTask = resourceLoader.LoadDocumentAsync(CenterPages.GetDepartments(center.Id))
            .WithAnonymousSession(cancellationToken)
            .AsTask();

        var undergraduateProgramsDocumentTask = resourceLoader.LoadDocumentAsync(
                CenterPages.GetUndergraduatePrograms(center.Id))
            .WithAnonymousSession(cancellationToken)
            .AsTask();

        var postgraduateProgramsDocumentTask = resourceLoader.LoadDocumentAsync(
                CenterPages.GetPostgraduatePrograms(center.Id))
            .WithAnonymousSession(cancellationToken)
            .AsTask();

        var researchesDocumentTask = resourceLoader.LoadDocumentAsync(CenterPages.GetResearches(center.Id))
            .WithAnonymousSession(cancellationToken)
            .AsTask();

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

        var details = scrapingEngine.Scrape<CenterDetails>(centerDocument);

        var departmentsTask = scrapingEngine.ScrapeAllAsync<Department>(departmentsDocument, cancellationToken);

        var undergraduateProgramsTask = scrapingEngine.ScrapeAllAsync<UndergraduateProgram>(
            undergraduateProgramsDocument,
            cancellationToken);

        var postgraduateProgramsTask = scrapingEngine.ScrapeAllAsync<GraduateProgram>(
            postgraduateProgramsDocument,
            cancellationToken);

        var researchesTask = scrapingEngine.ScrapeAllAsync<Research>(researchesDocument, cancellationToken);

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