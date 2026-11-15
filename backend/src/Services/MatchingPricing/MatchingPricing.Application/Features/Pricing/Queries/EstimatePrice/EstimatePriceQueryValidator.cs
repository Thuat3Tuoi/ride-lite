using FluentValidation;
using MatchingPricing.Domain.Errors;

namespace MatchingPricing.Application.Features.Pricing.Queries.EstimatePrice;

public sealed class EstimatePriceQueryValidator : AbstractValidator<EstimatePriceQuery>
{
    public EstimatePriceQueryValidator()
    {
        RuleFor(x => x.PickupLatitude)
            .InclusiveBetween(-90m, 90m)
            .WithState(_ => MatchingPricingErrors.InvalidCoordinates);

        RuleFor(x => x.PickupLongitude)
            .InclusiveBetween(-180m, 180m)
            .WithState(_ => MatchingPricingErrors.InvalidCoordinates);

        RuleFor(x => x.DropoffLatitude)
            .InclusiveBetween(-90m, 90m)
            .WithState(_ => MatchingPricingErrors.InvalidCoordinates);

        RuleFor(x => x.DropoffLongitude)
            .InclusiveBetween(-180m, 180m)
            .WithState(_ => MatchingPricingErrors.InvalidCoordinates);
    }
}

