using FluentValidation;
using Trip.Domain.Errors;

namespace Trip.Application.Features.Trip.Commands.DriverArrive;

public sealed class DriverArriveCommandValidator : AbstractValidator<DriverArriveCommand>
{
    public DriverArriveCommandValidator()
    {
        RuleFor(x => x.TripId)
            .GreaterThan(0)
            .WithState(_ => TripErrors.NotFound);
    }
}

