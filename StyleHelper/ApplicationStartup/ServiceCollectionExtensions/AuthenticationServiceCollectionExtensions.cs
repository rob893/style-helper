using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RHerber.Common.AspNetCore.Core;
using RHerber.Common.AspNetCore.Models.Settings;
using StyleHelper.Constants;
using StyleHelper.Services;

namespace StyleHelper.ApplicationStartup.ServiceCollectionExtensions;

public static class AuthenticationServiceCollectionExtensions
{
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration config)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (config == null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.Configure<AuthenticationSettings>(config.GetSection(ConfigurationKeys.Authentication));

        var authSettings = config.GetSection(ConfigurationKeys.Authentication).Get<AuthenticationSettings>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // Set token validation options. These will be used when validating all tokens.
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authSettings.APISecrect)),
                    RequireSignedTokens = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ValidAudience = authSettings.TokenAudience,
                    ValidIssuers = new string[] { authSettings.TokenIssuer }
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var errorMessage = string.IsNullOrWhiteSpace(context.ErrorDescription) ? context.Error : $"{context.Error}. {context.ErrorDescription}.";

                        var problem = new ProblemDetailsWithErrors(errorMessage ?? "Invalid token", StatusCodes.Status401Unauthorized, context.Request);

                        var settings = new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        };

                        return context.Response.WriteAsync(JsonConvert.SerializeObject(problem, settings));
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        var problem = new ProblemDetailsWithErrors("Forbidden", StatusCodes.Status403Forbidden, context.Request);

                        var settings = new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        };

                        return context.Response.WriteAsync(JsonConvert.SerializeObject(problem, settings));
                    },
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            context.Response.Headers.Add(AppHeaderNames.TokenExpired, "true");
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationPolicyName.RequireAdminRole, policy => policy.RequireRole(UserRoleName.Admin));
            options.AddPolicy(AuthorizationPolicyName.RequireUserRole, policy => policy.RequireRole(UserRoleName.User));
        });

        return services;
    }
}
