using FluentValidation;
using Trip.Domain.Errors;

namespace Trip.Application.Features.Trip.Commands.MarkNoDriverFound;

public sealed class MarkNoDriverFoundCommandValidator : AbstractValidator<MarkNoDriverFoundCommand>
{
    public MarkNoDriverFoundCommandValidator()
    {
        RuleFor(x => x.TripId)
            .GreaterThan(0)
            .WithState(_ => TripErrors.NotFound);
    }
}

