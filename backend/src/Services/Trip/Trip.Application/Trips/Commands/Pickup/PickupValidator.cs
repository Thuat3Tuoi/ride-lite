using Trip.Application.Abstractions.Validation;
using Trip.Application.Exceptions;

namespace Trip.Application.Trips.Commands.Pickup;

public sealed class PickupValidator : IValidator<PickupCommand>
{
    public void Validate(PickupCommand command)
    {
        if (command.TripId <= 0)
        {
            throw new TripValidationException("Trip id must be positive.");
        }
    }
}
