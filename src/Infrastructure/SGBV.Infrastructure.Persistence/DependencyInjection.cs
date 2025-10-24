using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGBV.Infrastructure.Persistence.Context;

namespace SGBV.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext(configuration);

        return services;
    }

    private static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SgbvContext>(postgres =>
        {
            postgres.UseNpgsql(configuration.GetConnectionString("SgbvConnection"),
                options => options.MigrationsAssembly("SGBV.Infrastructure.Persistence"));
        });
    }
}