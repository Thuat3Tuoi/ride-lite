namespace MatchingPricing.Application.Features.Assignment.Commands.RejectAssignment;

public sealed record RejectAssignmentCommand(
    long AssignmentId,
    long DriverId
);

