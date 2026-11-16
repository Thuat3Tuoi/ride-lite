using MatchingPricing.Application.Abstractions.Pricing;
using MatchingPricing.Domain.Enumerators;

namespace MatchingPricing.Application.Features.Pricing.Factory;

/// <summary>
/// Factory pattern implementation for managing and resolving PricingStrategy instances.
/// </summary>
public sealed class PricingStrategyFactory : IPricingStrategyFactory
{
    private readonly IReadOnlyDictionary<PricingMethod, IPricingStrategy> _pricingStrategies;

    public PricingStrategyFactory(IEnumerable<IPricingStrategy> pricingStrategies)
    {
        _pricingStrategies = pricingStrategies.ToDictionary(s => s.PricingMethod);
    }

    public IPricingStrategy GetPricingStrategy(PricingMethod pricingMethod)
    {
        if (_pricingStrategies.TryGetValue(pricingMethod, out var strategy))
        {
            return strategy;
        }

        throw new KeyNotFoundException($"Pricing strategy for method '{pricingMethod}' was not found.");
    }
}

