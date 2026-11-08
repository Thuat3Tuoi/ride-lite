using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Trip.Application.Abstractions;
using Trip.Application.Services;
using Trip.Infrastructure.Clients;
using Trip.Infrastructure.Messaging;
using Trip.Infrastructure.Payments;
using Trip.Infrastructure.Persistence;

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
        services.AddScoped<TripService>();

        services.AddSingleton<IMatchingClient, FakeMatchingClient>();
        services.AddSingleton<IPaymentGateway, FakePaymentGateway>();
        services.AddSingleton<IEventBus, InMemoryEventBus>();

        return services;
    }
}
