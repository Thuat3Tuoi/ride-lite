using Trip.Application.Abstractions.Validation;
using Trip.Application.Exceptions;

namespace Trip.Application.Trips.Commands.ConfirmQuote;

public sealed class ConfirmQuoteValidator : IValidator<ConfirmQuoteCommand>
{
    public void Validate(ConfirmQuoteCommand command)
    {
        if (command.TripId <= 0)
        {
            throw new TripValidationException("Trip id must be positive.");
        }
    }
}
