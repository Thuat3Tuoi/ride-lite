namespace Trip.Application.Features.Trip.Commands.StartTrip;

public sealed record StartTripResult(
    long Id,
    string Status,
    DateTimeOffset StartedAt,
    DateTimeOffset UpdatedAt
);

