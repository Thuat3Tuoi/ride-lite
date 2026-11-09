namespace Trip.Application.Features.Trip.Commands.CompleteTrip;

public sealed record CompleteTripResult(
    long Id,
    string Status,
    DateTimeOffset CompletedAt,
    DateTimeOffset UpdatedAt
);

