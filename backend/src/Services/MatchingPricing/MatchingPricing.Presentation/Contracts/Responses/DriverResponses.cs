namespace MatchingPricing.Presentation.Contracts.Responses;

public sealed record DriverResponse(
    long Id,
    string FullName,
    string PhoneNumber,
    DateOnly? DateOfBirth,
    decimal? Latitude,
    decimal? Longitude,
    string? Location,
    string Status,
    DateTimeOffset? LocationUpdatedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);

public sealed record NearbyDriverResponse(
    DriverResponse Driver,
    double DistanceKm
);

