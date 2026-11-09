using Trip.Application.Features.Trip.Queries.GetTripById;

namespace Trip.Application.Features.Trip.Queries.GetTripsByStatus;

public sealed record GetTripsByStatusResult(
    IReadOnlyList<GetTripByIdResult> Items
);

