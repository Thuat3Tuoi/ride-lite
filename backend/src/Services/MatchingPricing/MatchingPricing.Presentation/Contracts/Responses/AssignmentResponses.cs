namespace MatchingPricing.Presentation.Contracts.Responses;

public sealed record DriverAssignmentResponse(
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

