using Microsoft.EntityFrameworkCore;
using RHerber.Common.AspNetCore.Models.Settings;
using StyleHelper.Constants;
using StyleHelper.Data;

namespace StyleHelper.ApplicationStartup.ServiceCollectionExtensions;

public static class DatabaseServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration config)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (config == null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        services.Configure<MySQLSettings>(config.GetSection(ConfigurationKeys.MySQL));

        var settings = config.GetSection(ConfigurationKeys.MySQL).Get<MySQLSettings>();

        services.AddDbContext<DataContext>(
            dbContextOptions =>
            {
                dbContextOptions
                    .UseMySql(settings.DefaultConnection, ServerVersion.AutoDetect(settings.DefaultConnection), options =>
                    {
                        options.EnableRetryOnFailure();
                        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    });

                if (settings.EnableDetailedErrors)
                {
                    dbContextOptions.EnableDetailedErrors();
                }

                if (settings.EnableSensitiveDataLogging)
                {
                    dbContextOptions.EnableSensitiveDataLogging();
                }
            }
        );

        services.AddTransient<IDatabaseSeeder, DatabaseSeeder>();

        return services;
    }
}
