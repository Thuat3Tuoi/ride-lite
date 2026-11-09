using Trip.Application.Abstractions.Validation;
using Trip.Application.Exceptions;

namespace Trip.Application.Trips.Commands.Dropoff;

public sealed class DropoffValidator : IValidator<DropoffCommand>
{
    public void Validate(DropoffCommand command)
    {
        if (command.TripId <= 0)
        {
            throw new TripValidationException("Trip id must be positive.");
        }
    }
}
