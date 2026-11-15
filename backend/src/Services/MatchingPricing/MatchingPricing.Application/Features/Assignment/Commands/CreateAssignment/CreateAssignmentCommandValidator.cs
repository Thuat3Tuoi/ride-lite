using FluentValidation;
using MatchingPricing.Domain.Errors;

namespace MatchingPricing.Application.Features.Assignment.Commands.CreateAssignment;

public sealed class CreateAssignmentCommandValidator : AbstractValidator<CreateAssignmentCommand>
{
    public CreateAssignmentCommandValidator()
    {
        RuleFor(x => x.TripId)
            .GreaterThan(0)
            .WithState(_ => MatchingPricingErrors.InvalidTripId);

        RuleFor(x => x.DriverId)
            .GreaterThan(0)
            .WithState(_ => MatchingPricingErrors.InvalidDriverId);

        RuleFor(x => x.OfferDuration)
            .Must(d => !d.HasValue || d.Value > TimeSpan.Zero)
            .WithState(_ => MatchingPricingErrors.InvalidParameters);
    }
}

