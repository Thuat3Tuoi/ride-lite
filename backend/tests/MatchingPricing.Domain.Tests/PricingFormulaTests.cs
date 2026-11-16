using MatchingPricing.Domain.Pricing;
using Xunit;

namespace MatchingPricing.Domain.Tests;

public class PricingFormulaTests
{
    [Fact]
    public void CalculateTripPrice_RegularValues_ShouldCalculateCorrectly()
    {
        // Formula: ((cd * d + ct * t) * s + cm) * eta * (1 + T)
        // distanceCost = 10000 * 5 = 50000
        // timeCost = 1000 * 10 = 10000
        // basePrice = 60000
        // surgedPrice = 60000 * 1.0 = 60000
        // priceWithAdditionalFee = 60000 + 5000 = 65000
        // discountedPrice = 65000 * 1.0 = 65000
        // finalPrice = 65000 * 1.10 = 71500
        var price = PriceFormula.CalculateTripPrice(
            costPerDistance: 10000m,
            distance: 5m,
            costPerTime: 1000m,
            estimatedTime: 10m,
            surgeMultiplier: 1.0m,
            additionalFee: 5000m,
            discountFactor: 1.0m,
            taxRate: 0.10m
        );

        Assert.Equal(71500m, price);
    }

    [Fact]
    public void CalculateTripPrice_SurgeMultiplier_ShouldScalePrice()
    {
        // basePrice = (10000 * 5) + (1000 * 10) = 60000
        // surgedPrice = 60000 * 1.5 = 90000
        // priceWithFee = 90000 + 0 = 90000
        // discounted = 90000 * 1.0 = 90000
        // finalPrice = 90000 * 1.10 = 99000
        var price = PriceFormula.CalculateTripPrice(
            costPerDistance: 10000m,
            distance: 5m,
            costPerTime: 1000m,
            estimatedTime: 10m,
            surgeMultiplier: 1.5m,
            additionalFee: 0m,
            discountFactor: 1.0m,
            taxRate: 0.10m
        );

        Assert.Equal(99000m, price);
    }

    [Fact]
    public void CalculateTripPrice_DiscountFactor_ShouldDiscountPrice()
    {
        // basePrice = 60000
        // surgedPrice = 60000 * 1.0 = 60000
        // priceWithFee = 60000
        // discountedPrice = 60000 * 0.8 = 48000
        // finalPrice = 48000 * 1.10 = 52800
        var price = PriceFormula.CalculateTripPrice(
            costPerDistance: 10000m,
            distance: 5m,
            costPerTime: 1000m,
            estimatedTime: 10m,
            surgeMultiplier: 1.0m,
            additionalFee: 0m,
            discountFactor: 0.8m,
            taxRate: 0.10m
        );

        Assert.Equal(52800m, price);
    }

    [Fact]
    public void CalculateTripPrice_ZeroParameters_ShouldReturnZero()
    {
        var price = PriceFormula.CalculateTripPrice(
            costPerDistance: 0m,
            distance: 0m,
            costPerTime: 0m,
            estimatedTime: 0m,
            surgeMultiplier: 1.0m,
            additionalFee: 0m,
            discountFactor: 1.0m,
            taxRate: 0m
        );

        Assert.Equal(0m, price);
    }
}

