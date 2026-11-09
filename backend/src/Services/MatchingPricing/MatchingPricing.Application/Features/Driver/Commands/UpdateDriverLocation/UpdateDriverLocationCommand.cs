namespace MatchingPricing.Application.Features.Driver.Commands.UpdateDriverLocation;

public sealed record UpdateDriverLocationCommand(
    long DriverId,
    decimal Latitude,
    decimal Longitude
);

