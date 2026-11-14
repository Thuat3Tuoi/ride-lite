namespace MatchingPricing.Presentation.Contracts.Responses;

public sealed record EstimatePriceResponse(
    decimal Fare,
    double DistanceKm,
    string Currency
);

