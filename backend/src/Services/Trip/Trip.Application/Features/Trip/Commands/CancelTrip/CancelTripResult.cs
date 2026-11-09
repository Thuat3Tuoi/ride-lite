namespace Trip.Application.Features.Trip.Commands.CancelTrip;

public sealed record CancelTripResult(
    long Id,
    string Status,
    string CancelledBy,
    string? CancelReason,
    DateTimeOffset CancelledAt,
    DateTimeOffset UpdatedAt
);

