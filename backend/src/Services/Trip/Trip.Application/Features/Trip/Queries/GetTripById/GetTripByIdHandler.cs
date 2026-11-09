using BuildingBlocks.SharedKernel.Exceptions;
using Trip.Application.Abstractions.Data;
using Trip.Domain.Errors;

namespace Trip.Application.Features.Trip.Queries.GetTripById;

public sealed class GetTripByIdHandler
{
    private readonly ITripRepository _tripRepository;

    public GetTripByIdHandler(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task<GetTripByIdResult> HandleAsync(
        GetTripByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var trip = await _tripRepository.GetByIdAsync(query.Id, cancellationToken);
        if (trip is null)
        {
            throw new AppException(TripErrors.NotFound);
        }

        return new GetTripByIdResult(
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
            trip.UpdatedAt,
            trip.AcceptedAt,
            trip.DriverArrivedAt,
            trip.StartedAt,
            trip.CompletedAt,
            trip.CancelledAt,
            trip.CancelledBy?.ToString(),
            trip.CancelReason
        );
    }
}

