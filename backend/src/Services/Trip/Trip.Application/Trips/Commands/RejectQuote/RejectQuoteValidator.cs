using Trip.Application.Abstractions.Validation;
using Trip.Application.Exceptions;

namespace Trip.Application.Trips.Commands.RejectQuote;

public sealed class RejectQuoteValidator : IValidator<RejectQuoteCommand>
{
    public void Validate(RejectQuoteCommand command)
    {
        if (command.TripId <= 0)
        {
            throw new TripValidationException("Trip id must be positive.");
        }
    }
}
