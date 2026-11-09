namespace Trip.Application.Features.Rider.Queries.GetRiders;

public sealed record GetRidersQuery(int Page = 1, int PageSize = 20);

