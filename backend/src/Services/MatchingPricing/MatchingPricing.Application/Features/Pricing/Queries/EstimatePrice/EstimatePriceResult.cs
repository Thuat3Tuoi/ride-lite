namespace MatchingPricing.Application.Features.Pricing.Queries.EstimatePrice;

public sealed record EstimatePriceResult(
    decimal Fare,
    double DistanceKm,
    string Currency = "VND"
);

