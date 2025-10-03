using System.Reflection;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace Sigapi.Common.Problems;

public sealed class NotFoundProblem : IResult, IEndpointMetadataProvider, IStatusCodeHttpResult,
    IContentTypeHttpResult, IValueHttpResult, IValueHttpResult<ProblemDetails>
{
    private readonly ProblemHttpResult problem;

    public NotFoundProblem(string message)
    {
        problem = TypedResults.Problem
        (
            statusCode: StatusCode,
            title: "Not Found",
            detail: message
        );
    }

    public int? StatusCode => StatusCodes.Status404NotFound;

    public string ContentType => problem.ContentType;

    public object Value => problem.ProblemDetails;

    ProblemDetails IValueHttpResult<ProblemDetails>.Value => problem.ProblemDetails;

    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        var metadata = new ProducesResponseTypeMetadata(
            StatusCodes.Status404NotFound,
            typeof(ProblemDetails),
            ["application/problem+json"]);

        builder.Metadata.Add(metadata);
    }

    public Task ExecuteAsync(HttpContext context) => problem.ExecuteAsync(context);
}