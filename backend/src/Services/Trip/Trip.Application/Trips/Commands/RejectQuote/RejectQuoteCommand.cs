namespace Trip.Application.Trips.Commands.RejectQuote;

public sealed record RejectQuoteCommand(long TripId, string? Reason);
