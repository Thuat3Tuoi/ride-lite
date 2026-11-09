namespace MatchingPricing.Application.Features.Driver.Commands.UpdateDriverLocation;

public sealed record UpdateDriverLocationResult(
    long DriverId,
    decimal Latitude,
    decimal Longitude,
    string Location,
    DateTimeOffset LocationUpdatedAt
);

