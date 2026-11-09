namespace Trip.Application.Trips.Commands.DriverReject;

public sealed record DriverRejectCommand(long TripId, long DriverId);
