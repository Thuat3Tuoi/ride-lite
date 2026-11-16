using MatchingPricing.Application.Abstractions.Pricing;
using MatchingPricing.Domain.Enumerators;
using MatchingPricing.Domain.Pricing;

namespace MatchingPricing.Application.Features.Pricing.Strategies;

public sealed class CalculateSurgePriceStrategy : IPricingStrategy
{
    public PricingMethod PricingMethod => PricingMethod.Surge;

    public decimal CalculateTripFare(
        decimal costPerDistance,
        decimal distance,
        decimal costPerTime,
        decimal estimatedTime,
        decimal additionalFee,
        decimal taxRate)
    {
        return PriceFormula.CalculateTripPrice(
            costPerDistance,
            distance,
            costPerTime,
            estimatedTime,
            surgeMultiplier: 1.5m,
            additionalFee: additionalFee,
            discountFactor: 1.0m,
            taxRate: taxRate
        );
    }
}

