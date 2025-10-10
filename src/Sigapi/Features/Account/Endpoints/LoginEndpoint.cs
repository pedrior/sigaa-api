using Sigapi.Common.Endpoints;
using Sigapi.Common.Extensions;
using Sigapi.Common.RateLimiter;
using Sigapi.Common.Security.Tokens;
using Sigapi.Features.Account.Contracts;
using Sigapi.Features.Account.Exceptions;
using Sigapi.Features.Account.Models;
using Sigapi.Features.Account.Scraping;
using Sigapi.Scraping.Browsing;
using Sigapi.Scraping.Browsing.Sessions;
using Sigapi.Scraping.Document;
using Sigapi.Scraping.Engine;

namespace Sigapi.Features.Account.Endpoints;

[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed class LoginEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder route)
    {
        route.MapPost("/login", HandleAsync)
            .RequireRateLimiting(RateLimiterPolicies.Account.SessionManagement)
            .WithRequestValidation<LoginRequest>()
            .WithSummary("Login")
            .WithDescription("Autentica um estudante usando suas credenciais e retorna um token de acesso para " +
                             "requisições futuras.")
            .Accepts<LoginRequest>("application/json")
            .Produces<LoginResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    internal sealed class RequestValidator : AbstractValidator<LoginRequest>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Must not be null or empty.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Must not be null or empty.");

            RuleFor(x => x.Enrollment)
                .Must(v => string.IsNullOrEmpty(v) || v.All(char.IsDigit))
                .WithMessage("Must be a valid enrollment identifier consisting of digits only.");
        }
    }

    private static async Task<IResult> HandleAsync(LoginRequest request,
        HttpContext context,
        IResourceLoader resourceLoader,
        IScrapingEngine scrapingEngine,
        IEnumerable<ILoginResponseHandler> responseHandlers,
        ISecurityTokenProvider securityTokenProvider,
        ISessionManager sessionManager,
        CancellationToken cancellationToken)
    {
        var session = sessionManager.CreateSession();

        var loginDocument = await resourceLoader.LoadDocumentAsync(AccountPages.Login)
            .WithSession(session, cancellationToken);

        var loginForm = scrapingEngine.Scrape<LoginForm>(loginDocument);
        var loginFormData = loginForm.PrepareForSubmission(request.Username, request.Password);
        var loginResponseDocument = await resourceLoader.LoadDocumentAsync(loginForm.Action)
            .WithFormData(loginFormData)
            .WithSession(session, cancellationToken);

        var user = await HandleLoginResponseAsync(
            session,
            loginResponseDocument,
            responseHandlers,
            request.Enrollment,
            cancellationToken);

        await sessionManager.SaveSessionAsync(session, cancellationToken);

        var response = CreateAccessToken(
            session.Id,
            request.Username,
            user.Enrollment,
            user.Enrollments,
            securityTokenProvider);

        return Results.Ok(response);
    }

    private static Task<User> HandleLoginResponseAsync(ISession session,
        IDocument document,
        IEnumerable<ILoginResponseHandler> handlers,
        string? enrollment = null,
        CancellationToken cancellationToken = default)
    {
        var handler = handlers.FirstOrDefault(strategy => strategy.Evaluate(document));
        return handler is null
            ? throw new LoginException($"No login response handler found for page {document.Url}.")
            : handler.HandleAsync(session, document, enrollment, cancellationToken);
    }

    private static LoginResponse CreateAccessToken(string sessionId,
        string username,
        Enrollment enrollment,
        IEnumerable<Enrollment> enrollments,
        ISecurityTokenProvider securityTokenProvider)
    {
        var claims = new Dictionary<string, object>
        {
            [CustomClaimTypes.Username] = username,
            [CustomClaimTypes.Enrollment] = enrollment.Number,
            [CustomClaimTypes.Enrollments] = enrollments.OrderByDescending(e => e.Number)
                .Select(e => e.Number)
                .ToArray(),
            [JwtRegisteredClaimNames.Sid] = sessionId
        };

        var (token, expiresAt) = securityTokenProvider.CreateToken(claims);
        return new LoginResponse
        {
            Token = token,
            ExpiresAt = expiresAt
        };
    }
}