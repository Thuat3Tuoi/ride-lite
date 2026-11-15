using FluentValidation;
using MatchingPricing.Domain.Errors;

namespace MatchingPricing.Application.Features.Assignment.Commands.RejectAssignment;

public sealed class RejectAssignmentCommandValidator : AbstractValidator<RejectAssignmentCommand>
{
    public RejectAssignmentCommandValidator()
    {
        RuleFor(x => x.AssignmentId)
            .GreaterThan(0)
            .WithState(_ => MatchingPricingErrors.InvalidAssignmentId);

        RuleFor(x => x.DriverId)
            .GreaterThan(0)
            .WithState(_ => MatchingPricingErrors.InvalidDriverId);
    }
}

