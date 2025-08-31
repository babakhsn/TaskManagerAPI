using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Application.Abstractions;
using TaskManager.Infrastructure.Identity;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var conn = config.GetConnectionString("DefaultConnection")!;
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(conn));

        // Identity
        services.AddIdentityCore<ApplicationUser>(opt =>
        {
            opt.User.RequireUniqueEmail = true;
            opt.Password.RequireDigit = true;
            opt.Password.RequireUppercase = false;
            opt.Password.RequireLowercase = true;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequiredLength = 6;
        })
        .AddRoles<IdentityRole>()              // optional, but handy
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddSignInManager()                    // this registers SignInManager<ApplicationUser>
        .AddDefaultTokenProviders();

        // JWT Auth
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        services.AddAuthorization(); // roles/policies later

        // App abstractions
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
