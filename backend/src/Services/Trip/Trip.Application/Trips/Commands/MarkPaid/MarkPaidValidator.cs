using Trip.Application.Abstractions.Validation;
using Trip.Application.Exceptions;

namespace Trip.Application.Trips.Commands.MarkPaid;

public sealed class MarkPaidValidator : IValidator<MarkPaidCommand>
{
    public void Validate(MarkPaidCommand command)
    {
        if (command.TripId <= 0)
        {
            throw new TripValidationException("Trip id must be positive.");
        }
    }
}
