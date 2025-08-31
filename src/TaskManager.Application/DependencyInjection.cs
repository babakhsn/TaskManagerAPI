using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace TaskManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));
        // (FluentValidation scanner can also live here later)
        return services;
    }
}
