using MatchingPricing.Domain.Enumerators;

namespace MatchingPricing.Presentation.Contracts.Requests;

public sealed record UpdateDriverStatusRequest(
    DriverStatus Status
);

