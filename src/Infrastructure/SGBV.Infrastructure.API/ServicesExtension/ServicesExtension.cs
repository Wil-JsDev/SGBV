using SGBV.Infrastructure.API.Filters;
using SGBV.Infrastructure.API.Middlewares;

namespace SGBV.Infrastructure.API.ServicesExtension;

public static class ServicesExtension
{
    public static void AddFilters(this IMvcBuilder builder)
    {
        builder.AddMvcOptions(options => { options.Filters.Add<ResultFilter>(); });
    }
    
    public static void UseGlobalException(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<GlobalException>();
    }
}