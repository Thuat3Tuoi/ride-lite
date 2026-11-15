namespace MatchingPricing.Presentation.Contracts.Responses;

public sealed record NearbyDriverResponse(
    DriverResponse Driver,
    double DistanceKm
);

