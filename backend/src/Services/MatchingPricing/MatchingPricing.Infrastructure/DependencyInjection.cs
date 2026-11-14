using MatchingPricing.Application.Abstractions.Data;
using MatchingPricing.Application.Abstractions.Pricing;
using MatchingPricing.Application.Features.Assignment.Commands.AcceptAssignment;
using MatchingPricing.Application.Features.Assignment.Commands.CreateAssignment;
using MatchingPricing.Application.Features.Assignment.Commands.RejectAssignment;
using MatchingPricing.Application.Features.Assignment.Queries.GetAssignmentsByTripId;
using MatchingPricing.Application.Features.Driver.Commands.CreateDriver;
using MatchingPricing.Application.Features.Driver.Commands.UpdateDriverLocation;
using MatchingPricing.Application.Features.Driver.Commands.UpdateDriverStatus;
using MatchingPricing.Application.Features.Driver.Queries.FindNearbyDrivers;
using MatchingPricing.Application.Features.Driver.Queries.GetDriverById;
using MatchingPricing.Application.Features.Pricing.Queries.EstimatePrice;
using MatchingPricing.Infrastructure.Data.DataContext;
using MatchingPricing.Infrastructure.Data.Repositories;
using MatchingPricing.Infrastructure.Services.Pricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MatchingPricing.Infrastructure;

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
        services.AddScoped<IDriverRepository, DriverRepository>();
        services.AddScoped<IDriverAssignmentRepository, DriverAssignmentRepository>();

        // Pricing Calculator
        services.AddScoped<IPricingCalculator, PricingCalculator>();

        // Handlers
        services.AddMatchingPricingHandlers();

        return services;
    }

    private static IServiceCollection AddMatchingPricingHandlers(this IServiceCollection services)
    {
        // Driver Handlers
        services.AddScoped<CreateDriverHandler>();
        services.AddScoped<UpdateDriverLocationHandler>();
        services.AddScoped<UpdateDriverStatusHandler>();
        services.AddScoped<GetDriverByIdHandler>();
        services.AddScoped<FindNearbyDriversHandler>();

        // Assignment Handlers
        services.AddScoped<CreateAssignmentHandler>();
        services.AddScoped<AcceptAssignmentHandler>();
        services.AddScoped<RejectAssignmentHandler>();
        services.AddScoped<GetAssignmentsByTripIdHandler>();

        // Pricing Handlers
        services.AddScoped<EstimatePriceHandler>();

        return services;
    }
}

