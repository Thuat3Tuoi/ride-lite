namespace Trip.Application.Features.Trip.Commands.DriverArrive;

public sealed record DriverArriveResult(
    long Id,
    string Status,
    DateTimeOffset DriverArrivedAt,
    DateTimeOffset UpdatedAt
);

