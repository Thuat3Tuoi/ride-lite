namespace MatchingPricing.Application.Features.Assignment.Commands.CreateAssignment;

public sealed record CreateAssignmentCommand(
    long TripId,
    long DriverId,
    TimeSpan? OfferDuration = null
);

