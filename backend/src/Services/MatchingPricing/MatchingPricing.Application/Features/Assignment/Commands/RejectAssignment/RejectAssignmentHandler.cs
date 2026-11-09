using BuildingBlocks.SharedKernel.Exceptions;
using MatchingPricing.Application.Abstractions.Data;
using MatchingPricing.Domain.Errors;

namespace MatchingPricing.Application.Features.Assignment.Commands.RejectAssignment;

public sealed class RejectAssignmentHandler
{
    private readonly IDriverAssignmentRepository _assignmentRepository;

    public RejectAssignmentHandler(IDriverAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }

    public async Task<RejectAssignmentResult> HandleAsync(
        RejectAssignmentCommand command,
        CancellationToken cancellationToken = default)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(command.AssignmentId, cancellationToken);
        if (assignment is null)
        {
            throw new AppException(MatchingPricingErrors.AssignmentNotFound);
        }

        if (assignment.DriverId != command.DriverId)
        {
            throw new AppException(MatchingPricingErrors.DriverMismatch);
        }

        try
        {
            assignment.Reject();
        }
        catch (InvalidOperationException)
        {
            throw new AppException(MatchingPricingErrors.InvalidAssignmentStatusTransition);
        }

        await _assignmentRepository.SaveChangesAsync(cancellationToken);

        return new RejectAssignmentResult(
            assignment.Id,
            assignment.TripId,
            assignment.DriverId,
            assignment.Status.ToString(),
            assignment.RespondedAt!.Value,
            assignment.UpdatedAt
        );
    }
}

