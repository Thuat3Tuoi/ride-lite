namespace MatchingPricing.Application.Features.Assignment.Commands.AcceptAssignment;

public sealed record AcceptAssignmentResult(
    long Id,
    long TripId,
    long DriverId,
    string Status,
    DateTimeOffset RespondedAt,
    DateTimeOffset UpdatedAt
);

