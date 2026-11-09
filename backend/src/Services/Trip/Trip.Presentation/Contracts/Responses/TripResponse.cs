namespace Trip.Presentation.Contracts.Responses;

public sealed record TripResponse(
    long Id,
    long? DriverId,
    long RiderId,
    string PickupAddress,
    decimal PickupLatitude,
    decimal PickupLongitude,
    string DropoffAddress,
    decimal DropoffLatitude,
    decimal DropoffLongitude,
    decimal Cost,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? AcceptedAt = null,
    DateTimeOffset? DriverArrivedAt = null,
    DateTimeOffset? StartedAt = null,
    DateTimeOffset? CompletedAt = null,
    DateTimeOffset? CancelledAt = null,
    string? CancelledBy = null,
    string? CancelReason = null
);

