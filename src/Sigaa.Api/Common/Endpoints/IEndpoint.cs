namespace Sigaa.Api.Common.Endpoints;

internal interface IEndpoint
{
    static abstract void Map(IEndpointRouteBuilder route);
}