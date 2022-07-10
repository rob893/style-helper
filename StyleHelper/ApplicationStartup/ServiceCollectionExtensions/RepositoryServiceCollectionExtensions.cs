using StyleHelper.Data.Repositories;

namespace StyleHelper.ApplicationStartup.ServiceCollectionExtensions;

public static class RepositoryServiceCollectionExtensions
{
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
