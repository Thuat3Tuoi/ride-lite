namespace Trip.Domain.DomainEvents;

public sealed record TripAccepted(
    long TripId,
    long DriverId,
    DateTime AcceptedAt) : IDomainEvent;
