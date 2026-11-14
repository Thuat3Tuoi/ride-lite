using MatchingPricing.Domain.Enumerators;

namespace MatchingPricing.Application.Features.Driver.Commands.UpdateDriverStatus;

public sealed record UpdateDriverStatusCommand(
    long DriverId,
    DriverStatus Status
);

