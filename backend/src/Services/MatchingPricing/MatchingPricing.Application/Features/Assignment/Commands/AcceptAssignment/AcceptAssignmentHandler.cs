using BuildingBlocks.SharedKernel.Exceptions;
using MatchingPricing.Application.Abstractions.Data;
using MatchingPricing.Domain.Enumerators;
using MatchingPricing.Domain.Errors;

namespace MatchingPricing.Application.Features.Assignment.Commands.AcceptAssignment;

public sealed class AcceptAssignmentHandler
{
    private readonly IDriverAssignmentRepository _assignmentRepository;
    private readonly IDriverRepository _driverRepository;

    public AcceptAssignmentHandler(
        IDriverAssignmentRepository assignmentRepository,
        IDriverRepository driverRepository)
    {
        _assignmentRepository = assignmentRepository;
        _driverRepository = driverRepository;
    }

    public async Task<AcceptAssignmentResult> HandleAsync(
        AcceptAssignmentCommand command,
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

        // Concurrency control: prevent multiple accepted assignments for the same trip
        var alreadyAccepted = await _assignmentRepository.GetAcceptedAssignmentForTripAsync(assignment.TripId, cancellationToken);
        if (alreadyAccepted is not null && alreadyAccepted.Id != assignment.Id)
        {
            throw new AppException(MatchingPricingErrors.TripAlreadyAssigned);
        }

        try
        {
            assignment.Accept();
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("expired", StringComparison.OrdinalIgnoreCase))
        {
            await _assignmentRepository.SaveChangesAsync(cancellationToken);
            throw new AppException(MatchingPricingErrors.AssignmentExpired);
        }
        catch (InvalidOperationException)
        {
            throw new AppException(MatchingPricingErrors.InvalidAssignmentStatusTransition);
        }

        // Update winning driver to Busy
        var driver = await _driverRepository.GetByIdAsync(assignment.DriverId, cancellationToken);
        driver?.SetBusy();

        // Cancel other pending offers for this trip
        var otherAssignments = await _assignmentRepository.GetByTripIdAsync(assignment.TripId, cancellationToken);
        foreach (var other in otherAssignments)
        {
            if (other.Id != assignment.Id && other.Status == AssignmentStatus.OFFERED)
            {
                other.Cancel();
            }
        }

        await _assignmentRepository.SaveChangesAsync(cancellationToken);

        return new AcceptAssignmentResult(
            assignment.Id,
            assignment.TripId,
            assignment.DriverId,
            assignment.Status.ToString(),
            assignment.RespondedAt!.Value,
            assignment.UpdatedAt
        );
    }
}

