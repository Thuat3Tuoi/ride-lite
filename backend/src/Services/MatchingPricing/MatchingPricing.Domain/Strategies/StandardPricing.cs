using MatchingPricing.Domain.ValueObjects;

namespace MatchingPricing.Domain.Strategies;

// Gia thuong chi giu don gia, toll va thue tu cau hinh server
public sealed class StandardPricing : IPricingStrategy
{
    public FareParameters GetParameters(PricingContext context, FareParameters parameters)
        => new(parameters.DistanceRate, parameters.TimeRate, tollFee: parameters.TollFee, taxRate: parameters.TaxRate);
}
