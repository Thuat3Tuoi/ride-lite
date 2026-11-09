using BuildingBlocks.SharedKernel.Exceptions;
using Trip.Application.Abstractions.Data;
using Trip.Domain.Errors;

namespace Trip.Application.Features.Trip.Commands.StartTrip;

public sealed class StartTripHandler
{
    private readonly ITripRepository _tripRepository;

    public StartTripHandler(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task<StartTripResult> HandleAsync(
        StartTripCommand command,
        CancellationToken cancellationToken = default)
    {
        var trip = await _tripRepository.GetByIdAsync(command.TripId, cancellationToken);
        if (trip is null)
        {
            throw new AppException(TripErrors.NotFound);
        }

        try
        {
            trip.StartTrip();
        }
        catch (InvalidOperationException)
        {
            throw new AppException(TripErrors.InvalidStatusTransition);
        }

        await _tripRepository.SaveChangesAsync(cancellationToken);

        return new StartTripResult(
            trip.Id,
            trip.Status.ToString(),
            trip.StartedAt!.Value,
            trip.UpdatedAt
        );
    }
}

