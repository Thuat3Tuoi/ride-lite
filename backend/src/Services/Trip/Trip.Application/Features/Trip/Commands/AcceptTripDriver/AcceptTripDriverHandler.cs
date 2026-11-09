using BuildingBlocks.SharedKernel.Exceptions;
using Trip.Application.Abstractions.Data;
using Trip.Domain.Errors;

namespace Trip.Application.Features.Trip.Commands.AcceptTripDriver;

public sealed class AcceptTripDriverHandler
{
    private readonly ITripRepository _tripRepository;

    public AcceptTripDriverHandler(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task<AcceptTripDriverResult> HandleAsync(
        AcceptTripDriverCommand command,
        CancellationToken cancellationToken = default)
    {
        var trip = await _tripRepository.GetByIdAsync(command.TripId, cancellationToken);
        if (trip is null)
        {
            throw new AppException(TripErrors.NotFound);
        }

        try
        {
            trip.AcceptDriver(command.DriverId);
        }
        catch (InvalidOperationException)
        {
            throw new AppException(TripErrors.InvalidStatusTransition);
        }
        catch (ArgumentException)
        {
            throw new AppException(TripErrors.InvalidDriverId);
        }

        await _tripRepository.SaveChangesAsync(cancellationToken);

        return new AcceptTripDriverResult(
            trip.Id,
            trip.DriverId!.Value,
            trip.Status.ToString(),
            trip.AcceptedAt!.Value,
            trip.UpdatedAt
        );
    }
}

