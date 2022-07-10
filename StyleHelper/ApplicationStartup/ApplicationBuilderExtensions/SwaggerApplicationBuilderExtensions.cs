using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using RHerber.Common.AspNetCore.Extensions;
using RHerber.Common.AspNetCore.Middleware;

namespace StyleHelper.ApplicationStartup.ApplicationBuilderExtensions;

public static class SwaggerApplicationBuilderExtensions
{
    public static IApplicationBuilder UseAndConfigureSwagger(this IApplicationBuilder app, IConfiguration config)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        if (config == null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        app.UseMiddleware<SwaggerBasicAuthMiddleware>()
            .UseSwagger()
            .UseSwaggerUI(
                options =>
                {
                    var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();

                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.RoutePrefix = "swagger";
                        options.SwaggerEndpoint(
                            $"{description.GroupName}/swagger.json",
                            $"{FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductName} {description.GroupName}");
                        options.DocumentTitle = $"WoW Market Watcher - {config.GetEnvironment()}";
                    }
                });

        return app;
    }
}
