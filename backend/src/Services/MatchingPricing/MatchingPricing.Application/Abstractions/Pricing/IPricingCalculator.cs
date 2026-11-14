namespace MatchingPricing.Application.Abstractions.Pricing;

public interface IPricingCalculator
{
    decimal CalculateFare(
        decimal pickupLatitude,
        decimal pickupLongitude,
        decimal dropoffLatitude,
        decimal dropoffLongitude);
}

