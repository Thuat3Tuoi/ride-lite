using Trip.Domain.ValueObjects;

namespace Trip.Domain.DomainEvents;

public sealed record TripRequested(
    long TripId,
    long RiderId,
    GeoPoint Pickup,
    GeoPoint Dropoff,
    DateTime RequestedAt) : IDomainEvent;
