using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Trip.Application.Abstractions.Data;
using Trip.Application.Abstractions.Messaging;
using Trip.Application.Abstractions.Services;
using Trip.Application.Abstractions.Validation;
using Trip.Domain.Repositories;
using Trip.Infrastructure.Data.DataContext;
using Trip.Infrastructure.Data.Repositories;
using Trip.Infrastructure.Messaging;
using Trip.Infrastructure.Services;

namespace Trip.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTripInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<TripDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<TripRepository>();
        services.AddScoped<ITripReader>(sp => sp.GetRequiredService<TripRepository>());
        services.AddScoped<ITripWriter>(sp => sp.GetRequiredService<TripRepository>());
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        services.AddSingleton<IMatchingClient, FakeMatchingClient>();
        services.AddSingleton<IPaymentGateway, FakePaymentGateway>();
        services.AddSingleton<IEventBus, InMemoryEventBus>();

        services.AddApplication();

        return services;
    }

    private static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = typeof(ICommandHandler<,>).Assembly;
        var handlerDefinitions = new[]
        {
            typeof(ICommandHandler<,>),
            typeof(IQueryHandler<,>)
        };
        var validatorDefinition = typeof(IValidator<>);

        foreach (var implementation in applicationAssembly.GetTypes()
                     .Where(type => type is { IsClass: true, IsAbstract: false }))
        {
            foreach (var abstraction in implementation.GetInterfaces()
                         .Where(@interface => @interface.IsGenericType))
            {
                var definition = abstraction.GetGenericTypeDefinition();

                if (handlerDefinitions.Contains(definition) || definition == validatorDefinition)
                {
                    services.AddScoped(abstraction, implementation);
                }
            }
        }

        return services;
    }
}
