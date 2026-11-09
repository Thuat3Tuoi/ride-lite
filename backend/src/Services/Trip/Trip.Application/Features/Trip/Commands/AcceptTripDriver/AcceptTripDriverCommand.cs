namespace Trip.Application.Features.Trip.Commands.AcceptTripDriver;

public sealed record AcceptTripDriverCommand(
    long TripId,
    long DriverId
);

