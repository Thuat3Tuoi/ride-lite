using Trip.Application.Features.Rider.Queries.GetRiderById;

namespace Trip.Application.Features.Rider.Queries.GetRiders;

public sealed record GetRidersResult(
    IReadOnlyList<GetRiderByIdResult> Items,
    int Page,
    int PageSize
);

