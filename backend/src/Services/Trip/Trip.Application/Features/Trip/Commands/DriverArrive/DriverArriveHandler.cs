using BuildingBlocks.SharedKernel.Exceptions;
using Trip.Application.Abstractions.Data;
using Trip.Application.Abstractions.Messaging;
using Trip.Domain.Errors;

namespace Trip.Application.Features.Trip.Commands.DriverArrive;

public sealed class DriverArriveHandler : ICommandHandler<DriverArriveCommand, DriverArriveResult>
{
    private readonly ITripRepository _tripRepository;

    public DriverArriveHandler(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task<DriverArriveResult> HandleAsync(
        DriverArriveCommand command,
        CancellationToken cancellationToken = default)
    {
        var trip = await _tripRepository.GetByIdAsync(command.TripId, cancellationToken);
        if (trip is null)
        {
            throw new AppException(TripErrors.NotFound);
        }

        try
        {
            trip.DriverArrive();
        }
        catch (InvalidOperationException)
        {
            throw new AppException(TripErrors.InvalidStatusTransition);
        }

        await _tripRepository.SaveChangesAsync(cancellationToken);

        return new DriverArriveResult(
            trip.Id,
            trip.Status.ToString(),
            trip.DriverArrivedAt!.Value,
            trip.UpdatedAt
        );
    }
}
