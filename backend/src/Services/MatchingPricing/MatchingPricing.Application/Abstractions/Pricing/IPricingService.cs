using MatchingPricing.Domain.Enumerators;

namespace MatchingPricing.Application.Abstractions.Pricing;

/// <summary>
/// Service abstraction for calculating trip prices utilizing configured pricing strategies.
/// </summary>
public interface IPricingService
{
    decimal CalculateTripPrice(
        decimal costPerDistance,
        decimal distance,
        decimal costPerTime,
        decimal estimatedTime,
        decimal additionalFee,
        decimal taxRate,
        PricingMethod pricingMethod);
}

