using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.HttpOverrides;
using RHerber.Common.AspNetCore.Middleware;
using RHerber.Common.AspNetCore.Services;
using StyleHelper.ApplicationStartup.ApplicationBuilderExtensions;
using StyleHelper.ApplicationStartup.ServiceCollectionExtensions;
using StyleHelper.Core;
using StyleHelper.Middleware;

namespace StyleHelper.ApplicationStartup;

public sealed class Startup
{
    private readonly IConfiguration configuration;

    public Startup(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the DI container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllerServices()
            .AddLogging()
            .AddApplicationInsightsTelemetry()
            .AddDatabaseServices(this.configuration)
            .AddAuthenticationServices(this.configuration)
            .AddIdentityServices()
            .AddRepositoryServices()
            .AddSingleton<ITelemetryInitializer, ApplicationInsightsTelemetryInitializer>()
            .AddScoped<ICorrelationIdService, CorrelationIdService>()
            .AddSwaggerServices(this.configuration)
            .AddAutoMapper(typeof(Startup))
            .AddHealthCheckServices()
            .AddMemoryCache()
            .AddCors();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (!env.IsProduction())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseExceptionHandler(builder => builder.UseMiddleware<GlobalExceptionHandlerMiddleware>())
            .UseHsts()
            .UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            })
            .UseMiddleware<PathBaseRewriterMiddleware>()
            .UseMiddleware<CorrelationIdMiddleware>()
            .UseRouting()
            .UseAndConfigureCors(this.configuration)
            .UseAuthentication()
            .UseAuthorization()
            .UseAndConfigureSwagger(this.configuration)
            .UseAndConfigureEndpoints(this.configuration);
    }
}
