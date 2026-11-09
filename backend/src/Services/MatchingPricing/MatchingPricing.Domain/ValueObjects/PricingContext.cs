namespace MatchingPricing.Domain.ValueObjects;

// He so theo chuyen chi duoc tao tu chinh sach da kiem soat o phia server
public sealed record PricingContext
{
    public decimal DistanceKm { get; }
    public decimal DurationMinutes { get; }
    public decimal SurgeMultiplier { get; }
    public decimal PromotionFactor { get; }

    public PricingContext(decimal distanceKm, decimal durationMinutes, decimal surgeMultiplier = 1m, decimal promotionFactor = 1m)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(distanceKm);
        ArgumentOutOfRangeException.ThrowIfNegative(durationMinutes);
        ArgumentOutOfRangeException.ThrowIfLessThan(surgeMultiplier, 1m);
        ArgumentOutOfRangeException.ThrowIfNegative(promotionFactor);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(promotionFactor, 1m);
        (DistanceKm, DurationMinutes, SurgeMultiplier, PromotionFactor) =
            (distanceKm, durationMinutes, surgeMultiplier, promotionFactor);
    }
}
