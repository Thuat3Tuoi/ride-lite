using MatchingPricing.Domain.Enumerators;

namespace MatchingPricing.Application.Abstractions.Pricing;

/// <summary>
/// Factory interface for retrieving the appropriate pricing strategy based on the pricing method.
/// </summary>
public interface IPricingStrategyFactory
{
    IPricingStrategy GetPricingStrategy(PricingMethod pricingMethod);
}

