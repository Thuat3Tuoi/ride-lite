namespace MatchingPricing.Domain;

// Chi kich hoat cac dieu chinh can thiet, khong thay doi StandardPricing
public sealed class PricingStrategyFactory
{
    public IPricingStrategy Create(PricingContext context)
    {
        IPricingStrategy strategy = new StandardPricing();
        if (context.SurgeMultiplier > 1m)
            strategy = new SurgePricing(strategy);
        if (context.PromotionFactor < 1m)
            strategy = new PromoPricing(strategy);
        return strategy;
    }
}
