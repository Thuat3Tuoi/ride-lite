namespace Trip.Application.Features.Trip.Commands.CreateTrip;

public sealed record CreateTripResult(
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
    DateTimeOffset UpdatedAt
);

