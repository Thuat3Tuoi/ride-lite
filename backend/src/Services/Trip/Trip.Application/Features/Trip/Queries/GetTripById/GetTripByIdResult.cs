namespace Trip.Application.Features.Trip.Queries.GetTripById;

public sealed record GetTripByIdResult(
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
    DateTimeOffset? AcceptedAt,
    DateTimeOffset? DriverArrivedAt,
    DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt,
    DateTimeOffset? CancelledAt,
    string? CancelledBy,
    string? CancelReason
);

