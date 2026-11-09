using BuildingBlocks.SharedKernel.Exceptions;
using Trip.Application.Abstractions.Data;
using Trip.Application.Abstractions.Pricing;
using Trip.Domain.Errors;
using TripEntity = Trip.Domain.Entities.Trip;

namespace Trip.Application.Features.Trip.Commands.CreateTrip;

public sealed class CreateTripHandler
{
    private readonly ITripRepository _tripRepository;
    private readonly IRiderRepository _riderRepository;
    private readonly IPricingService _pricingService;

    public CreateTripHandler(
        ITripRepository tripRepository,
        IRiderRepository riderRepository,
        IPricingService pricingService)
    {
        _tripRepository = tripRepository;
        _riderRepository = riderRepository;
        _pricingService = pricingService;
    }

    public async Task<CreateTripResult> HandleAsync(
        CreateTripCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Verify rider exists
        var rider = await _riderRepository.GetByIdAsync(command.RiderId, cancellationToken);
        if (rider is null)
        {
            throw new AppException(TripErrors.RiderNotFound);
        }

        // 2. Calculate/validate fare via Pricing Service (Authoritative fare, never trust client)
        var authoritativeCost = await _pricingService.CalculateFareAsync(
            command.PickupLatitude,
            command.PickupLongitude,
            command.DropoffLatitude,
            command.DropoffLongitude,
            cancellationToken
        );

        if (authoritativeCost < 0)
        {
            throw new AppException(TripErrors.InvalidCost);
        }

        // 3. Create domain entity
        var trip = TripEntity.Create(
            command.RiderId,
            command.PickupAddress,
            command.PickupLatitude,
            command.PickupLongitude,
            command.DropoffAddress,
            command.DropoffLatitude,
            command.DropoffLongitude,
            authoritativeCost
        );

        // 4. Save trip
        await _tripRepository.AddAsync(trip, cancellationToken);
        await _tripRepository.SaveChangesAsync(cancellationToken);

        return new CreateTripResult(
            trip.Id,
            trip.DriverId,
            trip.RiderId,
            trip.PickupAddress,
            trip.PickupLatitude,
            trip.PickupLongitude,
            trip.DropoffAddress,
            trip.DropoffLatitude,
            trip.DropoffLongitude,
            trip.Cost,
            trip.Status.ToString(),
            trip.CreatedAt,
            trip.UpdatedAt
        );
    }
}

