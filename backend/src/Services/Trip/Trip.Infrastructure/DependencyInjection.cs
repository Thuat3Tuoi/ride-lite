using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Trip.Application.Abstractions.Data;
using Trip.Application.Abstractions.Pricing;
using Trip.Application.Features.Rider.Commands.CreateRider;
using Trip.Application.Features.Rider.Commands.UpdateRiderProfile;
using Trip.Application.Features.Rider.Queries.GetRiderById;
using Trip.Application.Features.Rider.Queries.GetRiders;
using Trip.Application.Features.Trip.Commands.AcceptTripDriver;
using Trip.Application.Features.Trip.Commands.CancelTrip;
using Trip.Application.Features.Trip.Commands.CompleteTrip;
using Trip.Application.Features.Trip.Commands.CreateTrip;
using Trip.Application.Features.Trip.Commands.DriverArrive;
using Trip.Application.Features.Trip.Commands.MarkNoDriverFound;
using Trip.Application.Features.Trip.Commands.StartTrip;
using Trip.Application.Features.Trip.Queries.GetTripById;
using Trip.Application.Features.Trip.Queries.GetTripsByDriverId;
using Trip.Application.Features.Trip.Queries.GetTripsByRiderId;
using Trip.Application.Features.Trip.Queries.GetTripsByStatus;
using Trip.Infrastructure.Data.DataContext;
using Trip.Infrastructure.Data.Repositories;
using Trip.Infrastructure.Services.Pricing;

namespace Trip.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        var connectionString =
            configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException(
                "Database connection string is not configured.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Repositories
        services.AddScoped<IRiderRepository, RiderRepository>();
        services.AddScoped<ITripRepository, TripRepository>();

        // Pricing Service
        services.AddScoped<IPricingService, PricingService>();

        // Handlers
        services.AddTripHandlers();

        return services;
    }

    private static IServiceCollection AddTripHandlers(this IServiceCollection services)
    {
        // Rider Handlers
        services.AddScoped<CreateRiderHandler>();
        services.AddScoped<UpdateRiderProfileHandler>();
        services.AddScoped<GetRiderByIdHandler>();
        services.AddScoped<GetRidersHandler>();

        // Trip Handlers
        services.AddScoped<CreateTripHandler>();
        services.AddScoped<AcceptTripDriverHandler>();
        services.AddScoped<DriverArriveHandler>();
        services.AddScoped<StartTripHandler>();
        services.AddScoped<CompleteTripHandler>();
        services.AddScoped<CancelTripHandler>();
        services.AddScoped<MarkNoDriverFoundHandler>();
        services.AddScoped<GetTripByIdHandler>();
        services.AddScoped<GetTripsByRiderIdHandler>();
        services.AddScoped<GetTripsByDriverIdHandler>();
        services.AddScoped<GetTripsByStatusHandler>();

        return services;
    }
}

