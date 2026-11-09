namespace Trip.Application.Trips.Commands.DriverAccept;

public sealed record DriverAcceptCommand(long TripId, long DriverId);
