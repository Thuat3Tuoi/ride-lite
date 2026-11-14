namespace MatchingPricing.Application.Features.Driver.Queries.GetDriverById;

public sealed record GetDriverByIdResult(
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

