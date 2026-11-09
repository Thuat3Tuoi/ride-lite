using Trip.Application.Abstractions.Validation;
using Trip.Application.Exceptions;

namespace Trip.Application.Trips.Commands.DriverReject;

public sealed class DriverRejectValidator : IValidator<DriverRejectCommand>
{
    public void Validate(DriverRejectCommand command)
    {
        if (command.TripId <= 0)
        {
            throw new TripValidationException("Trip id must be positive.");
        }

        if (command.DriverId <= 0)
        {
            throw new TripValidationException("Driver id must be positive.");
        }
    }
}
