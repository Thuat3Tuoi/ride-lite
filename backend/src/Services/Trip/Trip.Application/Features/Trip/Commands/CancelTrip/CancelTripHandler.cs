using BuildingBlocks.SharedKernel.Exceptions;
using Trip.Application.Abstractions.Data;
using Trip.Domain.Enumerators;
using Trip.Domain.Errors;

namespace Trip.Application.Features.Trip.Commands.CancelTrip;

public sealed class CancelTripHandler
{
    private readonly ITripRepository _tripRepository;

    public CancelTripHandler(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task<CancelTripResult> HandleAsync(
        CancelTripCommand command,
        CancellationToken cancellationToken = default)
    {
        var trip = await _tripRepository.GetByIdAsync(command.TripId, cancellationToken);
        if (trip is null)
        {
            throw new AppException(TripErrors.NotFound);
        }

        if (trip.Status == TripStatus.COMPLETED)
        {
            throw new AppException(TripErrors.AlreadyCompleted);
        }

        if (trip.Status == TripStatus.CANCELLED)
        {
            throw new AppException(TripErrors.AlreadyCancelled);
        }

        try
        {
            trip.Cancel(command.CancelledBy, command.Reason);
        }
        catch (InvalidOperationException)
        {
            throw new AppException(TripErrors.InvalidStatusTransition);
        }

        await _tripRepository.SaveChangesAsync(cancellationToken);

        return new CancelTripResult(
            trip.Id,
            trip.Status.ToString(),
            trip.CancelledBy!.Value.ToString(),
            trip.CancelReason,
            trip.CancelledAt!.Value,
            trip.UpdatedAt
        );
    }
}

