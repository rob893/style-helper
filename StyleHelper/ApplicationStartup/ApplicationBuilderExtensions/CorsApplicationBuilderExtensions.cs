using StyleHelper.Constants;

namespace StyleHelper.ApplicationStartup.ApplicationBuilderExtensions;

public static class CorsApplicationBuilderExtensions
{
    public static IApplicationBuilder UseAndConfigureCors(this IApplicationBuilder app, IConfiguration config)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        if (config == null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        app.UseCors(header =>
            header.WithOrigins(config.GetSection(ConfigurationKeys.CorsAllowedOrigins).Get<string[]>() ?? new[] { "*" })
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders(config.GetSection(ConfigurationKeys.CorsExposedHeaders).Get<string[]>() ?? new[] { AppHeaderNames.TokenExpired, AppHeaderNames.CorrelationId }));

        return app;
    }
}
