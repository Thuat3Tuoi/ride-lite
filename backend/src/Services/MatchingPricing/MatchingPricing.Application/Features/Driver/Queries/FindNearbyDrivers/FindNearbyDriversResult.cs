namespace MatchingPricing.Application.Features.Driver.Queries.FindNearbyDrivers;

public sealed record FindNearbyDriversResult(
    IReadOnlyList<NearbyDriverItem> Drivers
);
