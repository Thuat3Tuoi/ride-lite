using FluentValidation;

namespace MatchingPricing.Application.Features.Pricing.Queries.CalculatePrice;

public sealed class CalculatePriceQueryValidator : AbstractValidator<CalculatePriceQuery>
{
    public CalculatePriceQueryValidator()
    {
        RuleFor(x => x.CostPerDistance)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Cost per distance must be non-negative.");

        RuleFor(x => x.Distance)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Distance must be non-negative.");

        RuleFor(x => x.CostPerTime)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Cost per time must be non-negative.");

        RuleFor(x => x.EstimatedTime)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Estimated time must be non-negative.");

        RuleFor(x => x.AdditionalFee)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Additional fee must be non-negative.");

        RuleFor(x => x.TaxRate)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Tax rate must be non-negative.");

        RuleFor(x => x.PricingMethod)
            .IsInEnum()
            .WithMessage("Invalid pricing method.");
    }
}

