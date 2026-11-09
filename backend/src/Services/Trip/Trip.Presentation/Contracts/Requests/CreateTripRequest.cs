namespace Trip.Presentation.Contracts.Requests;

public sealed record CreateTripRequest(
    long RiderId,
    string PickupAddress,
    decimal PickupLatitude,
    decimal PickupLongitude,
    string DropoffAddress,
    decimal DropoffLatitude,
    decimal DropoffLongitude,
    decimal? ClientFare = null
);

