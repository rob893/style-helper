namespace StyleHelper.ApplicationStartup.ServiceCollectionExtensions;

public static class HealthCheckServiceCollectionExtensions
{
    public static IServiceCollection AddHealthCheckServices(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddHealthChecks();

        return services;
    }
}
