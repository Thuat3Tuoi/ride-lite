using Trip.Domain.Enumerators;

namespace Trip.Application.Features.Trip.Queries.GetTripsByStatus;

public sealed record GetTripsByStatusQuery(TripStatus Status);

