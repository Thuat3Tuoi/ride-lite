using Trip.Domain.Enumerators;

namespace Trip.Application.Features.Trip.Commands.CancelTrip;

public sealed record CancelTripCommand(
    long TripId,
    CancelledBy CancelledBy,
    string? Reason = null
);

