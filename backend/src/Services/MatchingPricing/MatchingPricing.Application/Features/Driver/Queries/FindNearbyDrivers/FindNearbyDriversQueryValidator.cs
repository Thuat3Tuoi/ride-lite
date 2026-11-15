using FluentValidation;
using MatchingPricing.Domain.Errors;

namespace MatchingPricing.Application.Features.Driver.Queries.FindNearbyDrivers;

public sealed class FindNearbyDriversQueryValidator : AbstractValidator<FindNearbyDriversQuery>
{
    public FindNearbyDriversQueryValidator()
    {
        RuleFor(x => x.PickupLatitude)
            .InclusiveBetween(-90m, 90m)
            .WithState(_ => MatchingPricingErrors.InvalidCoordinates);

        RuleFor(x => x.PickupLongitude)
            .InclusiveBetween(-180m, 180m)
            .WithState(_ => MatchingPricingErrors.InvalidCoordinates);

        RuleFor(x => x.RadiusKm)
            .GreaterThan(0)
            .WithState(_ => MatchingPricingErrors.InvalidRadius);

        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .WithState(_ => MatchingPricingErrors.InvalidParameters);
    }
}

