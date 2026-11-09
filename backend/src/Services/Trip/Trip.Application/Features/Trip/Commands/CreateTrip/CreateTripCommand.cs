namespace Trip.Application.Features.Trip.Commands.CreateTrip;

public sealed record CreateTripCommand(
    long RiderId,
    string PickupAddress,
    decimal PickupLatitude,
    decimal PickupLongitude,
    string DropoffAddress,
    decimal DropoffLatitude,
    decimal DropoffLongitude,
    decimal? ClientFare = null
);

