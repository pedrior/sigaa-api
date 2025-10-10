namespace Sigapi.Common.Endpoints;

internal interface IEndpoint
{
    static abstract void Map(IEndpointRouteBuilder router);
}