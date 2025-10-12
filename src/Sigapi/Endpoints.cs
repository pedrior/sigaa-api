using Sigapi.Common.Endpoints;
using Sigapi.Features.Account.Endpoints;
using Sigapi.Features.Centers.Endpoints;
using Sigapi.Features.Departments.Endpoints;

namespace Sigapi;

internal static class Endpoints
{
    internal const string AuthAndProfileTag = "Autenticação e Perfil";
    internal const string AcademicCenterTag = "Centros Acadêmicos";
    internal const string AcademicDepartmentTag = "Departamentos Acadêmicos";
    
    public static void MapEndpoints(this WebApplication app)
    {
        var root = app.MapGroup(string.Empty);

        root.MapAccountEndpoints();
        root.MapCenterEndpoints();
        root.MapDepartmentEndpoints();
    }

    private static void MapAccountEndpoints(this IEndpointRouteBuilder route)
    {
        var group = route.MapGroup("account")
            .WithTags(AuthAndProfileTag);

        group.MapPublicGroup()
            .MapEndpoint<LoginEndpoint>();

        group.MapAuthorizedGroup()
            .MapEndpoint<LogoutEndpoint>()
            .MapEndpoint<GetProfileEndpoint>();
    }

    private static void MapCenterEndpoints(this IEndpointRouteBuilder route)
    {
        var group = route.MapPublicGroup("centers")
            .WithTags(AcademicCenterTag);

        group.MapEndpoint<GetCenterEndpoint>()
            .MapEndpoint<ListCentersEndpoint>();
    }
    
    private static void MapDepartmentEndpoints(this IEndpointRouteBuilder route)
    {
        var group = route.MapPublicGroup("departments")
            .WithTags(AcademicDepartmentTag);

        group.MapEndpoint<ListDepartmentsEndpoint>();
    }

    private static RouteGroupBuilder MapPublicGroup(this IEndpointRouteBuilder route, string? prefix = null) =>
        route.MapGroupWithDefaults(prefix ?? string.Empty)
            .AllowAnonymous();

    private static RouteGroupBuilder MapAuthorizedGroup(this IEndpointRouteBuilder route, string? prefix = null)
    {
        return route.MapGroupWithDefaults(prefix ?? string.Empty)
            .RequireAuthorization()
            .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static RouteGroupBuilder MapGroupWithDefaults(this IEndpointRouteBuilder route, string? prefix = null)
    {
        return route.MapGroup(prefix ?? string.Empty)
            .ProducesProblem(StatusCodes.Status429TooManyRequests)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesProblem(StatusCodes.Status503ServiceUnavailable);
    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder route)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(route);
        return route;
    }
}