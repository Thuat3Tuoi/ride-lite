using BuildingBlocks.SharedKernel.Exceptions;
using MatchingPricing.Application.Abstractions.Data;
using MatchingPricing.Domain.Entities;
using MatchingPricing.Domain.Enumerators;
using MatchingPricing.Domain.Errors;

namespace MatchingPricing.Application.Features.Assignment.Commands.CreateAssignment;

public sealed class CreateAssignmentHandler
{
    private readonly IDriverAssignmentRepository _assignmentRepository;
    private readonly IDriverRepository _driverRepository;

    public CreateAssignmentHandler(
        IDriverAssignmentRepository assignmentRepository,
        IDriverRepository driverRepository)
    {
        _assignmentRepository = assignmentRepository;
        _driverRepository = driverRepository;
    }

    public async Task<CreateAssignmentResult> HandleAsync(
        CreateAssignmentCommand command,
        CancellationToken cancellationToken = default)
    {
        var driver = await _driverRepository.GetByIdAsync(command.DriverId, cancellationToken);
        if (driver is null)
        {
            throw new AppException(MatchingPricingErrors.DriverNotFound);
        }

        if (driver.Status != DriverStatus.available)
        {
            throw new AppException(MatchingPricingErrors.DriverNotAvailable);
        }

        var existingAccepted = await _assignmentRepository.GetAcceptedAssignmentForTripAsync(command.TripId, cancellationToken);
        if (existingAccepted is not null)
        {
            throw new AppException(MatchingPricingErrors.TripAlreadyAssigned);
        }

        var assignment = DriverAssignment.Create(
            command.TripId,
            command.DriverId,
            command.OfferDuration
        );

        await _assignmentRepository.AddAsync(assignment, cancellationToken);
        await _assignmentRepository.SaveChangesAsync(cancellationToken);

        return new CreateAssignmentResult(
            assignment.Id,
            assignment.TripId,
            assignment.DriverId,
            assignment.Status.ToString(),
            assignment.OfferedAt,
            assignment.ExpiresAt,
            assignment.CreatedAt
        );
    }
}

