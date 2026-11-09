namespace Trip.Application.Features.Trip.Commands.AcceptTripDriver;

public sealed record AcceptTripDriverResult(
    long Id,
    long DriverId,
    string Status,
    DateTimeOffset AcceptedAt,
    DateTimeOffset UpdatedAt
);

