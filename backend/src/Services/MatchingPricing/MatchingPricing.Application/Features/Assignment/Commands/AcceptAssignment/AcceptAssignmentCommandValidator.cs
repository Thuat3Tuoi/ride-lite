using FluentValidation;
using MatchingPricing.Domain.Errors;

namespace MatchingPricing.Application.Features.Assignment.Commands.AcceptAssignment;

public sealed class AcceptAssignmentCommandValidator : AbstractValidator<AcceptAssignmentCommand>
{
    public AcceptAssignmentCommandValidator()
    {
        RuleFor(x => x.AssignmentId)
            .GreaterThan(0)
            .WithState(_ => MatchingPricingErrors.InvalidAssignmentId);

        RuleFor(x => x.DriverId)
            .GreaterThan(0)
            .WithState(_ => MatchingPricingErrors.InvalidDriverId);
    }
}

