namespace MatchingPricing.Application.Features.Assignment.Queries.GetAssignmentsByTripId;

public sealed record GetAssignmentsByTripIdResult(
    IReadOnlyList<AssignmentDto> Items
);

