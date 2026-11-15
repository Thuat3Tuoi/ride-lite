using FluentValidation;
using MatchingPricing.Domain.Errors;

namespace MatchingPricing.Application.Features.Driver.Commands.UpdateDriverLocation;

public sealed class UpdateDriverLocationCommandValidator : AbstractValidator<UpdateDriverLocationCommand>
{
    public UpdateDriverLocationCommandValidator()
    {
        RuleFor(x => x.DriverId)
            .GreaterThan(0)
            .WithState(_ => MatchingPricingErrors.InvalidDriverId);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90m, 90m)
            .WithState(_ => MatchingPricingErrors.InvalidCoordinates);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180m, 180m)
            .WithState(_ => MatchingPricingErrors.InvalidCoordinates);
    }
}

