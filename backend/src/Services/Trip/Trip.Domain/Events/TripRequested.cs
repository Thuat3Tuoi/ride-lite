using Trip.Domain.ValueObjects;

namespace Trip.Domain.Events;

public sealed record TripRequested(
    long TripId,
    long RiderId,
    GeoPoint Pickup,
    GeoPoint Dropoff,
    DateTime RequestedAt) : IDomainEvent;
