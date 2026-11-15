namespace MatchingPricing.Presentation.Contracts.Requests;

public sealed record UpdateDriverLocationRequest(
    decimal Latitude,
    decimal Longitude
);

