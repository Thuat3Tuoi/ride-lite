using FluentValidation;
using Trip.Domain.Errors;

namespace Trip.Application.Features.Trip.Commands.AcceptTripDriver;

public sealed class AcceptTripDriverCommandValidator : AbstractValidator<AcceptTripDriverCommand>
{
    public AcceptTripDriverCommandValidator()
    {
        RuleFor(x => x.TripId)
            .GreaterThan(0)
            .WithState(_ => TripErrors.NotFound);

        RuleFor(x => x.DriverId)
            .GreaterThan(0)
            .WithState(_ => TripErrors.InvalidDriverId);
    }
}

