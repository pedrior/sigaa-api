using Sigaa.Api.Common.Endpoints;
using Sigaa.Api.Features.Account.Endpoints;
using Sigaa.Api.Features.Centers.Endpoints;
using Sigaa.Api.Features.Departments.Endpoints;

namespace Sigaa.Api;

internal static class EndpointExtensions
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
        var group = route.MapGroup("conta")
            .WithTags(AuthAndProfileTag);

        group.MapPublicGroup()
            .MapEndpoint<LoginEndpoint>();

        group.MapAuthorizedGroup()
            .MapEndpoint<LogoutEndpoint>()
            .MapEndpoint<GetProfileEndpoint>();
    }

    private static void MapCenterEndpoints(this IEndpointRouteBuilder route)
    {
        var group = route.MapPublicGroup("centros")
            .WithTags(AcademicCenterTag);

        group.MapEndpoint<GetCenterEndpoint>()
            .MapEndpoint<ListCentersEndpoint>();
    }
    
    private static void MapDepartmentEndpoints(this IEndpointRouteBuilder route)
    {
        var group = route.MapPublicGroup("departamentos")
            .WithTags(AcademicDepartmentTag);

        group.MapEndpoint<GetDepartmentEndpoint>()
            .MapEndpoint<ListDepartmentsEndpoint>();
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