using Sigapi.Common.Cache;
using Sigapi.Common.Endpoints;
using Sigapi.Common.Problems;
using Sigapi.Features.Centers.Contracts;
using Sigapi.Features.Centers.Models;
using Sigapi.Features.Centers.Scraping;
using Sigapi.Scraping.Document;
using Sigapi.Scraping.Engine;
using Sigapi.Scraping.Networking;

namespace Sigapi.Features.Centers.Endpoints;

[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed class GetCenterEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder route)
    {
        route.MapGet("/{idOrSlug}", HandleAsync)
            .CacheOutput(CachePolicies.Centers.GetCenter)
            .WithSummary("Obter centro acadêmico por ID ou Slug")
            .WithDescription("Obtém informações detalhadas sobre um centro acadêmico específico a partir de seu ID " +
                             "ou slug (ex: 'centro-de-informatica').")
            .Produces<CenterResponse>();
    }

    private static async Task<IResult> HandleAsync(string idOrSlug,
        HttpContext context,
        IPageFetcher pageFetcher,
        IScrapingEngine scrapingEngine,
        CancellationToken cancellationToken)
    {
        if (CenterLookup.FindByIdOrSlug(idOrSlug) is not var (centerId, centerSlug))
        {
            return idOrSlug.All(char.IsDigit)
                ? new NotFoundProblem($"Center with ID '{idOrSlug}' was not found.")
                : new NotFoundProblem($"Center with slug '{idOrSlug}' was not found.");
        }

        var response = await GetCenterAsync(
            centerId,
            centerSlug,
            pageFetcher,
            scrapingEngine,
            cancellationToken);

        return Results.Ok(response);
    }

    private static async Task<CenterResponse> GetCenterAsync(string centerId,
        string centerSlug,
        IPageFetcher pageFetcher,
        IScrapingEngine scrapingEngine,
        CancellationToken cancellationToken)
    {
        var centerPageTask = FetchPageAsync(pageFetcher, CenterPages.GetCenter(centerId), cancellationToken);
        var departmentsPageTask = FetchPageAsync(
            pageFetcher,
            CenterPages.GetDepartments(centerId),
            cancellationToken);

        var undergraduateProgramsPageTask = FetchPageAsync(
            pageFetcher,
            CenterPages.GetUndergraduatePrograms(centerId),
            cancellationToken);

        var postgraduateProgramsPageTask = FetchPageAsync(
            pageFetcher,
            CenterPages.GetPostgraduatePrograms(centerId),
            cancellationToken);

        var researchesPageTask = FetchPageAsync(pageFetcher, CenterPages.GetResearches(centerId), cancellationToken);

        await Task.WhenAll(
            centerPageTask,
            departmentsPageTask,
            undergraduateProgramsPageTask,
            postgraduateProgramsPageTask,
            researchesPageTask);

        var centerPage = centerPageTask.Result;
        var departmentsPage = departmentsPageTask.Result;
        var undergraduateProgramsPage = undergraduateProgramsPageTask.Result;
        var postgraduateProgramsPage = postgraduateProgramsPageTask.Result;
        var researchesPage = researchesPageTask.Result;

        var center = scrapingEngine.Scrape<Center>(centerPage);

        var departmentsTask = scrapingEngine.ScrapeAllAsync<Department>(departmentsPage, cancellationToken);

        var undergraduateProgramsTask = scrapingEngine.ScrapeAllAsync<UndergraduateProgram>(
            undergraduateProgramsPage,
            cancellationToken);

        var postgraduateProgramsTask = scrapingEngine.ScrapeAllAsync<GraduateProgram>(
            postgraduateProgramsPage,
            cancellationToken);

        var researchesTask = scrapingEngine.ScrapeAllAsync<Research>(researchesPage, cancellationToken);

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

        return new CenterResponse
        {
            Id = centerId,
            Slug = centerSlug,
            Name = center.Name,
            Acronym = center.Acronym,
            Address = center.Address,
            Director = center.Director,
            Description = center.Description,
            LogoUrl = center.LogoUrl,
            Departments = departmentsResponse,
            Programs = programsResponse,
            Researches = researchResponses
        };
    }

    private static async Task<IHtmlElement> FetchPageAsync(IPageFetcher pageFetcher,
        string url,
        CancellationToken cancellationToken)
    {
        return await pageFetcher.FetchAndParseAsync(url, cancellationToken: cancellationToken);
    }
}