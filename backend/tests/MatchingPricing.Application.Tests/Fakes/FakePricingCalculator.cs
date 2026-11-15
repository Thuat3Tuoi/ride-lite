using MatchingPricing.Application.Abstractions.Pricing;

namespace MatchingPricing.Application.Tests.Fakes;

public sealed class FakePricingCalculator : IPricingCalculator
{
    public decimal FixedFare { get; set; } = 45000m;

    public decimal CalculateFare(decimal pickupLatitude, decimal pickupLongitude, decimal dropoffLatitude, decimal dropoffLongitude)
    {
        return FixedFare;
    }
}

