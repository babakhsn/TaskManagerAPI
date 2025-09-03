using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using TaskManager.Application.Mappings;

namespace TaskManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationForAPI(this IServiceCollection services)
    {
        // ✅ Manual AutoMapper registration
        var mapperConfig = new MapperConfiguration(
            cfg => { cfg.AddProfile<DomainToDtoProfile>(); },
            NullLoggerFactory.Instance
        );
        IMapper mapper = mapperConfig.CreateMapper();
        services.AddSingleton(mapper);

        // MediatR + FluentValidation as before
        services.AddMediatR(typeof(DependencyInjection).Assembly);
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
