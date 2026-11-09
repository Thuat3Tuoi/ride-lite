using BuildingBlocks.SharedKernel.Exceptions;
using Trip.Application.Abstractions.Data;
using Trip.Domain.Errors;

namespace Trip.Application.Features.Trip.Commands.CompleteTrip;

public sealed class CompleteTripHandler
{
    private readonly ITripRepository _tripRepository;

    public CompleteTripHandler(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task<CompleteTripResult> HandleAsync(
        CompleteTripCommand command,
        CancellationToken cancellationToken = default)
    {
        var trip = await _tripRepository.GetByIdAsync(command.TripId, cancellationToken);
        if (trip is null)
        {
            throw new AppException(TripErrors.NotFound);
        }

        try
        {
            trip.CompleteTrip();
        }
        catch (InvalidOperationException)
        {
            throw new AppException(TripErrors.InvalidStatusTransition);
        }

        await _tripRepository.SaveChangesAsync(cancellationToken);

        return new CompleteTripResult(
            trip.Id,
            trip.Status.ToString(),
            trip.CompletedAt!.Value,
            trip.UpdatedAt
        );
    }
}

