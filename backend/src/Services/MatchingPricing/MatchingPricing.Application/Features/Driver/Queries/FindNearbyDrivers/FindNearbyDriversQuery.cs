namespace MatchingPricing.Application.Features.Driver.Queries.FindNearbyDrivers;

public sealed record FindNearbyDriversQuery(
    decimal PickupLatitude,
    decimal PickupLongitude,
    double RadiusKm = 10.0,
    int Limit = 10
);

