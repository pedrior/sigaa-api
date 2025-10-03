using Microsoft.AspNetCore.Diagnostics;
using Sigapi.Features.Account.Exceptions;
using Sigapi.Scraping.Networking.Sessions;

namespace Sigapi.Common.Exceptions;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = GetStatusCodeForException(exception);
        await Results.Problem(statusCode: statusCode, title: title)
            .ExecuteAsync(context);

        return true;
    }

    private static (int code, string? title) GetStatusCodeForException(Exception exception) => exception switch
    {
        UnauthorizedAccessException or SessionExpiredException => GetStatus401Unauthorized(),
        OperationCanceledException => GetStatus499ClientClosedRequest(),
        HttpRequestException => GetStatus503ServiceUnavailable(),
        LoginException ex and (InvalidCredentialsException or InvalidEnrollmentException) =>
            GetStatus401Unauthorized(ex.Message),
        _ => GetStatus500InternalServerError()
    };

    private static (int code, string title) GetStatus401Unauthorized(string? message = null) =>
        (StatusCodes.Status401Unauthorized, message ?? "You are not authorized to access this resource.");

    private static (int, string) GetStatus499ClientClosedRequest() => (StatusCodes.Status499ClientClosedRequest,
        "The client closed the connection before the server could complete the response.");

    private static (int, string) GetStatus500InternalServerError() => (StatusCodes.Status500InternalServerError,
        "An unexpected error occurred while processing your request.");

    private static (int code, string? title) GetStatus503ServiceUnavailable() => (
        StatusCodes.Status503ServiceUnavailable, "The server is currently unable to handle the request.");
}