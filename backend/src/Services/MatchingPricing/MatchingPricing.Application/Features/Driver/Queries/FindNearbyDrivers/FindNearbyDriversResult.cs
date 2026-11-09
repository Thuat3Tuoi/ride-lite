using MatchingPricing.Application.Features.Driver.Queries.GetDriverById;

namespace MatchingPricing.Application.Features.Driver.Queries.FindNearbyDrivers;

public sealed record NearbyDriverItem(
    GetDriverByIdResult Driver,
    double DistanceKm
);

public sealed record FindNearbyDriversResult(
    IReadOnlyList<NearbyDriverItem> Drivers
);

