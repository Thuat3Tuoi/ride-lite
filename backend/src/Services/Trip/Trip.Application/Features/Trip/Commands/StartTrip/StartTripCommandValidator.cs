using FluentValidation;
using Trip.Domain.Errors;

namespace Trip.Application.Features.Trip.Commands.StartTrip;

public sealed class StartTripCommandValidator : AbstractValidator<StartTripCommand>
{
    public StartTripCommandValidator()
    {
        RuleFor(x => x.TripId)
            .GreaterThan(0)
            .WithState(_ => TripErrors.NotFound);
    }
}

