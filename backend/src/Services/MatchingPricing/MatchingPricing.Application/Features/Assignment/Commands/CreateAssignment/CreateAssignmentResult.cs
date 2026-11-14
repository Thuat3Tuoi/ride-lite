namespace MatchingPricing.Application.Features.Assignment.Commands.CreateAssignment;

public sealed record CreateAssignmentResult(
    long Id,
    long TripId,
    long DriverId,
    string Status,
    DateTimeOffset OfferedAt,
    DateTimeOffset ExpiresAt,
    DateTimeOffset CreatedAt
);

