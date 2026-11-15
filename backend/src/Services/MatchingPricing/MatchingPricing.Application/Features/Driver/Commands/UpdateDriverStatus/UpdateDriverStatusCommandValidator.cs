using FluentValidation;
using MatchingPricing.Domain.Errors;

namespace MatchingPricing.Application.Features.Driver.Commands.UpdateDriverStatus;

public sealed class UpdateDriverStatusCommandValidator : AbstractValidator<UpdateDriverStatusCommand>
{
    public UpdateDriverStatusCommandValidator()
    {
        RuleFor(x => x.DriverId)
            .GreaterThan(0)
            .WithState(_ => MatchingPricingErrors.InvalidDriverId);

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithState(_ => MatchingPricingErrors.InvalidParameters);
    }
}

