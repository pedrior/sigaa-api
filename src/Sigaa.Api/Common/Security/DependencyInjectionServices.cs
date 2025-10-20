using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Sigaa.Api.Common.Scraping.Client.Sessions;
using Sigaa.Api.Common.Scraping.Client.Sessions.Storages;
using Sigaa.Api.Common.Security.Tokens;

namespace Sigaa.Api.Common.Security;

internal static class DependencyInjectionServices
{
    public static WebApplicationBuilder AddSecurityServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddDataProtection();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IUserContext, UserContext>();
        builder.Services.AddTransient<ISecurityTokenProvider, SecurityTokenProvider>();

        var jwtConfig = builder.Configuration.GetRequiredSection(SecurityTokenOptions.SectionName);
        builder.Services.AddOptionsWithValidateOnStart<SecurityTokenOptions>()
            .Bind(jwtConfig)
            .ValidateDataAnnotations();

        var securityTokenOptions = jwtConfig.Get<SecurityTokenOptions>()!;
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuer = !string.IsNullOrEmpty(securityTokenOptions.Issuer),
                    ValidIssuer = securityTokenOptions.Issuer,
                    ValidateAudience = !string.IsNullOrEmpty(securityTokenOptions.Audience),
                    ValidAudience = securityTokenOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(securityTokenOptions.Key))
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var sessionId = context.Principal?.Claims
                            .First(x => x.Type is PersistentSessionStorage.SessionIdClaimType).Value;

                        if (sessionId is null)
                        {
                            context.Fail("Session ID not found");
                            return;
                        }

                        var sessionDetailsAccessor = context.HttpContext.RequestServices
                            .GetRequiredService<ISessionDetailsAccessor>();

                        var sessionDetails = await sessionDetailsAccessor.GetSessionDetailsAsync(sessionId,
                            context.HttpContext.RequestAborted);

                        if (sessionDetails is null || sessionDetails.IsExpired())
                        {
                            context.Fail("Session expired");
                        }
                    }
                };
            });

        builder.Services.AddAuthorization();

        return builder;
    }
}