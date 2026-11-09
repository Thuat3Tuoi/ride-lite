using Trip.Application.Abstractions.Validation;
using Trip.Application.Exceptions;

namespace Trip.Application.Trips.Commands.MarkPaymentFailed;

public sealed class MarkPaymentFailedValidator : IValidator<MarkPaymentFailedCommand>
{
    public void Validate(MarkPaymentFailedCommand command)
    {
        if (command.TripId <= 0)
        {
            throw new TripValidationException("Trip id must be positive.");
        }
    }
}
