namespace MatchingPricing.Domain;

// Domain khong doc cau hinh va khong phu thuoc ha tang
public static class FareFormula
{
    public static FareBreakdown Calculate(PricingContext context, FareParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(parameters);

        // Khong cat so le trung gian va de overflow bi tu choi thay vi tra gia sai
        checked
        {
            decimal distanceFare = parameters.DistanceRate * context.DistanceKm;
            decimal timeFare = parameters.TimeRate * context.DurationMinutes;
            decimal subtotal = distanceFare + timeFare;
            decimal surgedFare = subtotal * parameters.SurgeMultiplier;
            decimal surgeAmount = surgedFare - subtotal;
            decimal beforePromotion = surgedFare + parameters.TollFee;
            decimal afterPromotion = beforePromotion * parameters.PromotionFactor;
            decimal promotionDiscount = beforePromotion - afterPromotion;
            decimal taxAmount = afterPromotion * parameters.TaxRate;
            decimal totalFare = afterPromotion + taxAmount;

            // Phi cau duong khong chiu surge, khuyen mai duoc ap dung truoc thue
            return new(distanceFare, timeFare, subtotal, surgeAmount, parameters.TollFee,
                promotionDiscount, taxAmount, totalFare);
        }
    }
}
