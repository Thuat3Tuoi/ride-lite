using Trip.Domain.Enumerators;

namespace Trip.Presentation.Contracts.Requests;

public sealed record CancelTripRequest(
    CancelledBy CancelledBy,
    string? Reason = null
);

