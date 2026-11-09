namespace MatchingPricing.Application.Features.Driver.Commands.UpdateDriverStatus;

public sealed record UpdateDriverStatusResult(
    long DriverId,
    string Status,
    DateTimeOffset UpdatedAt
);

