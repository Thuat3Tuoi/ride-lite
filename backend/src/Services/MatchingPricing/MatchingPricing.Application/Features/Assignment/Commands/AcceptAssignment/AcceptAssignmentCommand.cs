namespace MatchingPricing.Application.Features.Assignment.Commands.AcceptAssignment;

public sealed record AcceptAssignmentCommand(
    long AssignmentId,
    long DriverId
);

