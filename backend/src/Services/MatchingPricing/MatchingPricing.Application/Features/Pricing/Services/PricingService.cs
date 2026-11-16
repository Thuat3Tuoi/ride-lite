using MatchingPricing.Application.Abstractions.Pricing;
using MatchingPricing.Domain.Enumerators;

namespace MatchingPricing.Application.Features.Pricing.Services;

/// <summary>
/// Pricing service coordinating between pricing strategy factory and execution.
/// </summary>
public sealed class PricingService : IPricingService
{
    private readonly IPricingStrategyFactory _pricingStrategyFactory;

    public PricingService(IPricingStrategyFactory pricingStrategyFactory)
    {
        _pricingStrategyFactory = pricingStrategyFactory;
    }

    public decimal CalculateTripPrice(
        decimal costPerDistance,
        decimal distance,
        decimal costPerTime,
        decimal estimatedTime,
        decimal additionalFee,
        decimal taxRate,
        PricingMethod pricingMethod)
    {
        var pricingStrategy = _pricingStrategyFactory.GetPricingStrategy(pricingMethod);

        return pricingStrategy.CalculateTripFare(
            costPerDistance,
            distance,
            costPerTime,
            estimatedTime,
            additionalFee,
            taxRate
        );
    }
}

