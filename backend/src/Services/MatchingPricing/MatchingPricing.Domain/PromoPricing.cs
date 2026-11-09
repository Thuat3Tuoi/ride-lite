namespace MatchingPricing.Domain;

// Cong thuc tap trung quyet dinh vi tri khuyen mai du decorator duoc ghep theo thu tu nao
public sealed class PromoPricing(IPricingStrategy basePricing) : IPricingStrategy
{
    public FareParameters GetParameters(PricingContext context, FareParameters parameters)
    {
        var basis = basePricing.GetParameters(context, parameters);
        return new(basis.DistanceRate, basis.TimeRate, basis.SurgeMultiplier,
            basis.TollFee, context.PromotionFactor, basis.TaxRate);
    }
}
