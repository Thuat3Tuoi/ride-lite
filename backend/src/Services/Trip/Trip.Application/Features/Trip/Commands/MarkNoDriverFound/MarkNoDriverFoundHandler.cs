using BuildingBlocks.SharedKernel.Exceptions;
using Trip.Application.Abstractions.Data;
using Trip.Domain.Errors;

namespace Trip.Application.Features.Trip.Commands.MarkNoDriverFound;

public sealed class MarkNoDriverFoundHandler
{
    private readonly ITripRepository _tripRepository;

    public MarkNoDriverFoundHandler(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task<MarkNoDriverFoundResult> HandleAsync(
        MarkNoDriverFoundCommand command,
        CancellationToken cancellationToken = default)
    {
        var trip = await _tripRepository.GetByIdAsync(command.TripId, cancellationToken);
        if (trip is null)
        {
            throw new AppException(TripErrors.NotFound);
        }

        try
        {
            trip.MarkNoDriverFound();
        }
        catch (InvalidOperationException)
        {
            throw new AppException(TripErrors.InvalidStatusTransition);
        }

        await _tripRepository.SaveChangesAsync(cancellationToken);

        return new MarkNoDriverFoundResult(
            trip.Id,
            trip.Status.ToString(),
            trip.UpdatedAt
        );
    }
}

