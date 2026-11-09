namespace MatchingPricing.Application.Features.Pricing.Queries.EstimatePrice;

public sealed record EstimatePriceQuery(
    decimal PickupLatitude,
    decimal PickupLongitude,
    decimal DropoffLatitude,
    decimal DropoffLongitude
);

public sealed record EstimatePriceResult(
    decimal Fare,
    double DistanceKm,
    string Currency = "VND"
);

