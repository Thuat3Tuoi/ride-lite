namespace MatchingPricing.Application.Features.Assignment.Commands.RejectAssignment;

public sealed record RejectAssignmentResult(
    long Id,
    long TripId,
    long DriverId,
    string Status,
    DateTimeOffset RespondedAt,
    DateTimeOffset UpdatedAt
);

