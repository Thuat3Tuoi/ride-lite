using FluentValidation;
using MatchingPricing.Domain.Errors;

namespace MatchingPricing.Application.Features.Assignment.Queries.GetAssignmentsByTripId;

public sealed class GetAssignmentsByTripIdQueryValidator : AbstractValidator<GetAssignmentsByTripIdQuery>
{
    public GetAssignmentsByTripIdQueryValidator()
    {
        RuleFor(x => x.TripId)
            .GreaterThan(0)
            .WithState(_ => MatchingPricingErrors.InvalidTripId);
    }
}

