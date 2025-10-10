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
            .WithSummary("Obter centro acadêmico por ID ou Slug")
            .WithDescription("Obtém informações detalhadas sobre um centro acadêmico específico a partir de seu ID " +
                             "ou slug (ex: 'centro-de-informatica').")
            .Produces<CenterDetailsResponse>();
    }

    private static async Task<IResult> HandleAsync(string idOrSlug,
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
        var centerListPage = await resourceLoader.LoadDocumentAsync(CenterPages.CenterList)
            .WithAnonymousSession(cancellationToken);

        var centers = await scrapingEngine.ScrapeAllAsync<Center>(centerListPage, cancellationToken);
        return centers.FirstOrDefault(c => c.Id == idOrSlug || c.Slug == idOrSlug);
    }

    private static async Task<CenterDetailsResponse> GetCenterDetailsAsync(Center center,
        IResourceLoader resourceLoader,
        IScrapingEngine scrapingEngine,
        CancellationToken cancellationToken)
    {
        var centerPageTask = resourceLoader.LoadDocumentAsync(CenterPages.GetCenter(center.Id))
            .WithAnonymousSession(cancellationToken)
            .AsTask();
        
        var departmentsPageTask = resourceLoader.LoadDocumentAsync(CenterPages.GetDepartments(center.Id))
            .WithAnonymousSession(cancellationToken)
            .AsTask();
        
        var undergraduateProgramsPageTask = resourceLoader.LoadDocumentAsync(CenterPages.GetUndergraduatePrograms(center.Id))
            .WithAnonymousSession(cancellationToken)
            .AsTask();
        
        var postgraduateProgramsPageTask = resourceLoader.LoadDocumentAsync(CenterPages.GetPostgraduatePrograms(center.Id))
            .WithAnonymousSession(cancellationToken)
            .AsTask();
        
        var researchesPageTask = resourceLoader.LoadDocumentAsync(CenterPages.GetResearches(center.Id))
            .WithAnonymousSession(cancellationToken)
            .AsTask();

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

        var details = scrapingEngine.Scrape<CenterDetails>(centerPage);

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