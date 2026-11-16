using MatchingPricing.Domain.Enumerators;

namespace MatchingPricing.Application.Abstractions.Pricing;

/// <summary>
/// Strategy interface for calculating trip fare based on specific pricing policies.
/// </summary>
public interface IPricingStrategy
{
    PricingMethod PricingMethod { get; }

    decimal CalculateTripFare(
        decimal costPerDistance,
        decimal distance,
        decimal costPerTime,
        decimal estimatedTime,
        decimal additionalFee,
        decimal taxRate
    );
}

