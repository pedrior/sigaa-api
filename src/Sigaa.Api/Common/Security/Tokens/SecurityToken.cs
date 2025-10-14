namespace Sigaa.Api.Common.Security.Tokens;

internal sealed record SecurityToken(string Token, DateTimeOffset ExpiresAt);