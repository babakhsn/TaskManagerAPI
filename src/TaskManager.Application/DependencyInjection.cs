using AutoMapper;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Mappings; // DomainToDtoProfile
using TaskManager.Application.Common.Behaviors;

namespace TaskManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // ✅ Scans the assembly that contains DomainToDtoProfile
        //services.AddAutoMapper(typeof(DomainToDtoProfile));

        services.AddMediatR(typeof(DependencyInjection).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        //services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        return services;
    }
}
