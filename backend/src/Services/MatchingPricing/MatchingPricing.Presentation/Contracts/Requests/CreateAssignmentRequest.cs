namespace MatchingPricing.Presentation.Contracts.Requests;

public sealed record CreateAssignmentRequest(
    long TripId,
    long DriverId,
    TimeSpan? OfferDuration = null
);

