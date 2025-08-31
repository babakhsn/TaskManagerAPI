using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Abstractions;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var conn = config.GetConnectionString("DefaultConnection")
                   ?? "Server=localhost;Database=TaskManagerDB;Trusted_Connection=True;TrustServerCertificate=True;";

        // SQL Server
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(conn));

        // For PostgreSQL, use:
        // services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(conn));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}
