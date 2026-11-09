using MatchingPricing.Domain.ValueObjects;

namespace MatchingPricing.Domain.Strategies;

// Decorator gan he so thay vi nhan ket qua da gom toll va thue
public sealed class SurgePricing(IPricingStrategy basePricing) : IPricingStrategy
{
    public FareParameters GetParameters(PricingContext context, FareParameters parameters)
    {
        var basis = basePricing.GetParameters(context, parameters);
        return new(basis.DistanceRate, basis.TimeRate, context.SurgeMultiplier,
            basis.TollFee, basis.PromotionFactor, basis.TaxRate);
    }
}
