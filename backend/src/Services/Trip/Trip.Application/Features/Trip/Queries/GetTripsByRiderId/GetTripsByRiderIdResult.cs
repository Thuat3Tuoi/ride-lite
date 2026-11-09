using Trip.Application.Features.Trip.Queries.GetTripById;

namespace Trip.Application.Features.Trip.Queries.GetTripsByRiderId;

public sealed record GetTripsByRiderIdResult(
    IReadOnlyList<GetTripByIdResult> Items
);

