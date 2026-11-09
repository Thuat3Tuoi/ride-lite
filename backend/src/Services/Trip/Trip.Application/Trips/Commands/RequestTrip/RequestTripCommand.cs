namespace Trip.Application.Trips.Commands.RequestTrip;

public sealed record RequestTripCommand(
    long RiderId,
    string PickupAddress,
    decimal PickupLatitude,
    decimal PickupLongitude,
    string DropoffAddress,
    decimal DropoffLatitude,
    decimal DropoffLongitude);
