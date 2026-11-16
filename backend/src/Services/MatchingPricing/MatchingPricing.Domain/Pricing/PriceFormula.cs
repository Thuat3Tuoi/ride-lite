namespace MatchingPricing.Domain.Pricing;

/// <summary>
/// Domain formula for calculating trip prices based on distance, time, surge, fees, discount, and tax.
/// Formula: p = ((c_d * d + c_t * t) * s + c_m) * eta * (1 + T)
/// </summary>
public static class PriceFormula
{
    public static decimal CalculateTripPrice(
        decimal costPerDistance,
        decimal distance,
        decimal costPerTime,
        decimal estimatedTime,
        decimal surgeMultiplier,
        decimal additionalFee,
        decimal discountFactor,
        decimal taxRate)
    {
        var distanceCost = costPerDistance * distance;
        var timeCost = costPerTime * estimatedTime;
        var basePrice = distanceCost + timeCost;
        var surgedPrice = basePrice * surgeMultiplier;
        var priceWithAdditionalFee = surgedPrice + additionalFee;
        var discountedPrice = priceWithAdditionalFee * discountFactor;
        var taxFactor = 1.0m + taxRate;
        var finalPrice = discountedPrice * taxFactor;

        return Math.Round(finalPrice, 0, MidpointRounding.AwayFromZero);
    }
}

