using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGBV.Application.Interfaces.Repositories;
using SGBV.Application.Interfaces.Services;
using SGBV.Application.Services;

namespace SGBV.Application;

public static class DependencyInjection
{
    public static void AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
    {
        #region Services

        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<IRegistrationService, RegistrationService>();
        services.AddScoped<IResourceService, ResourceService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ILoanService, LoanService>();

        #endregion
    }
}