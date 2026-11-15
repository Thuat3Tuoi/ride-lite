using FluentValidation;
using Trip.Domain.Errors;

namespace Trip.Application.Features.Trip.Commands.CancelTrip;

public sealed class CancelTripCommandValidator : AbstractValidator<CancelTripCommand>
{
    public CancelTripCommandValidator()
    {
        RuleFor(x => x.TripId)
            .GreaterThan(0)
            .WithState(_ => TripErrors.NotFound);

        RuleFor(x => x.Reason)
            .Must(r => string.IsNullOrEmpty(r) || r.Length <= 500)
            .When(x => !string.IsNullOrEmpty(x.Reason));
    }
}

