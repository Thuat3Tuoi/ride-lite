namespace Trip.Application.Trips.Commands.MarkPaymentFailed;

public sealed record MarkPaymentFailedCommand(long TripId, string? Reason);
