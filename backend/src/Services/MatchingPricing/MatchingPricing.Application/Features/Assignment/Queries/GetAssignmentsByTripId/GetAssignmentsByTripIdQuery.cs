namespace MatchingPricing.Application.Features.Assignment.Queries.GetAssignmentsByTripId;

public sealed record GetAssignmentsByTripIdQuery(long TripId);

public sealed record AssignmentDto(
    long Id,
    long TripId,
    long DriverId,
    string Status,
    DateTimeOffset OfferedAt,
    DateTimeOffset ExpiresAt,
    DateTimeOffset? RespondedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);

public sealed record GetAssignmentsByTripIdResult(
    IReadOnlyList<AssignmentDto> Items
);

