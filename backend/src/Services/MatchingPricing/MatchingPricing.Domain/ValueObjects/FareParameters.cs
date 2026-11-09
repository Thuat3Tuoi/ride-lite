namespace MatchingPricing.Domain.ValueObjects;

// Bat bien de khong the bo qua validation sau khi khoi tao
public sealed record FareParameters
{
    public decimal DistanceRate { get; }
    public decimal TimeRate { get; }
    public decimal SurgeMultiplier { get; }
    public decimal TollFee { get; }
    public decimal PromotionFactor { get; }
    public decimal TaxRate { get; }

    public FareParameters(decimal distanceRate, decimal timeRate, decimal surgeMultiplier = 1m,
        decimal tollFee = 0m, decimal promotionFactor = 1m, decimal taxRate = 0m)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(distanceRate);
        ArgumentOutOfRangeException.ThrowIfNegative(timeRate);
        ArgumentOutOfRangeException.ThrowIfLessThan(surgeMultiplier, 1m);
        ArgumentOutOfRangeException.ThrowIfNegative(tollFee);
        ArgumentOutOfRangeException.ThrowIfNegative(promotionFactor);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(promotionFactor, 1m);
        ArgumentOutOfRangeException.ThrowIfNegative(taxRate);
        (DistanceRate, TimeRate, SurgeMultiplier, TollFee, PromotionFactor, TaxRate) =
            (distanceRate, timeRate, surgeMultiplier, tollFee, promotionFactor, taxRate);
    }
}
