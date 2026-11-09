namespace MatchingPricing.Domain;

// Giu nguyen do chinh xac de Application chi lam tron tai buoc hoan thien
public sealed record FareBreakdown(decimal DistanceFare, decimal TimeFare, decimal Subtotal,
    decimal SurgeAmount, decimal TollFee, decimal PromotionDiscount, decimal TaxAmount, decimal TotalFare)
{
    public string Currency => "VND";
}
