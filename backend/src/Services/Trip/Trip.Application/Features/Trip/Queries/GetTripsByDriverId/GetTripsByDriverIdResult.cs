using Trip.Application.Features.Trip.Queries.GetTripById;

namespace Trip.Application.Features.Trip.Queries.GetTripsByDriverId;

public sealed record GetTripsByDriverIdResult(
    IReadOnlyList<GetTripByIdResult> Items
);

