using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rex.Infrastructure.Persistence.Repository;
using SGBV.Application.Interfaces.Repositories;
using SGBV.Application.Interfaces.Services;
using SGBV.Infrastructure.Persistence.Context;
using SGBV.Infrastructure.Persistence.Repository;
using SGBV.Infrastructure.Persistence.Services;

namespace SGBV.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext(configuration);

        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<ILoanRepository, LoanRepository>();
        services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddTransient<IResourceRepository, ResourceRepository>();
        services.AddTransient<ResourceRepository, ResourceRepository>();
        services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddTransient<IUserRoleService, UserRoleService>();
        
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