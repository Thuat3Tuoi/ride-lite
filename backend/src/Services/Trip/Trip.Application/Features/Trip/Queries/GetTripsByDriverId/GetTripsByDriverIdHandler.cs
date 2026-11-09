using Trip.Application.Abstractions.Data;
using Trip.Application.Features.Trip.Queries.GetTripById;

namespace Trip.Application.Features.Trip.Queries.GetTripsByDriverId;

public sealed class GetTripsByDriverIdHandler
{
    private readonly ITripRepository _tripRepository;

    public GetTripsByDriverIdHandler(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task<GetTripsByDriverIdResult> HandleAsync(
        GetTripsByDriverIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var trips = await _tripRepository.GetByDriverIdAsync(query.DriverId, cancellationToken);

        var items = trips.Select(t => new GetTripByIdResult(
            t.Id,
            t.DriverId,
            t.RiderId,
            t.PickupAddress,
            t.PickupLatitude,
            t.PickupLongitude,
            t.DropoffAddress,
            t.DropoffLatitude,
            t.DropoffLongitude,
            t.Cost,
            t.Status.ToString(),
            t.CreatedAt,
            t.UpdatedAt,
            t.AcceptedAt,
            t.DriverArrivedAt,
            t.StartedAt,
            t.CompletedAt,
            t.CancelledAt,
            t.CancelledBy?.ToString(),
            t.CancelReason
        )).ToList();

        return new GetTripsByDriverIdResult(items);
    }
}

