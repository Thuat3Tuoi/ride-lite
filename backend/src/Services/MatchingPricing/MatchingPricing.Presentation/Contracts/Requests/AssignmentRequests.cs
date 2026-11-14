namespace MatchingPricing.Presentation.Contracts.Requests;

public sealed record CreateAssignmentRequest(
    long TripId,
    long DriverId,
    TimeSpan? OfferDuration = null
);

public sealed record AcceptAssignmentRequest(
    long DriverId
);

public sealed record RejectAssignmentRequest(
    long DriverId
);

