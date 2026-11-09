using MatchingPricing.Domain;

namespace MatchingPricing.Domain.Tests;

// Doi chieu tung thanh phan voi bo du lieu minh hoa, khong coi day la bang gia chinh thuc
public class FareFormulaTests
{
    [Fact]
    public void Calculate_IllustrativeTrip_Returns137000Vnd()
    {
        var result = FareFormula.Calculate(new PricingContext(10m, 20m), new FareParameters(10000m, 1500m, tollFee: 7000m));
        Assert.Equal(new FareBreakdown(100000m, 30000m, 130000m, 0m, 7000m, 0m, 0m, 137000m), result);
        Assert.Equal("VND", result.Currency);
    }

    [Theory]
    [InlineData(0, 20, 37000)]
    [InlineData(10, 0, 107000)]
    [InlineData(0, 0, 7000)]
    public void Calculate_ZeroMetrics_AreValid(int distance, int duration, int expected)
    {
        var result = FareFormula.Calculate(new PricingContext(distance, duration), new FareParameters(10000m, 1500m, tollFee: 7000m));
        Assert.Equal(expected, result.TotalFare);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    public void Context_NegativeMetrics_AreRejected(int distance, int duration)
        => Assert.Throws<ArgumentOutOfRangeException>(() => new PricingContext(distance, duration));

    [Theory]
    [InlineData(-1, 1, 1, 0, 1, 0)]
    [InlineData(1, -1, 1, 0, 1, 0)]
    [InlineData(1, 1, 0, 0, 1, 0)]
    [InlineData(1, 1, 1, -1, 1, 0)]
    [InlineData(1, 1, 1, 0, -1, 0)]
    [InlineData(1, 1, 1, 0, 2, 0)]
    [InlineData(1, 1, 1, 0, 1, -1)]
    public void Parameters_InvalidValues_AreRejected(int distanceRate, int timeRate, int surge, int toll, int promo, int tax)
        => Assert.Throws<ArgumentOutOfRangeException>(() => new FareParameters(distanceRate, timeRate, surge, toll, promo, tax));

    [Fact]
    public void Calculate_Adjustments_PreserveDecimalPrecisionAndOrder()
    {
        var result = FareFormula.Calculate(new PricingContext(0.1m, 0.2m), new FareParameters(0.3m, 0.4m, 1.5m, 0.07m, 0.8m, 0.1m));
        Assert.Equal(new FareBreakdown(0.03m, 0.08m, 0.11m, 0.055m, 0.07m, 0.047m, 0.0188m, 0.2068m), result);
    }

    [Fact]
    public void Calculate_Overflow_IsRejected()
        => Assert.Throws<OverflowException>(() => FareFormula.Calculate(new PricingContext(2m, 0m), new FareParameters(decimal.MaxValue, 0m)));
}
