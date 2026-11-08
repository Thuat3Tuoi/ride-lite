namespace Trip.Domain.Events;

public sealed record TripAccepted(
    long TripId,
    long DriverId,
    DateTime AcceptedAt) : IDomainEvent;
