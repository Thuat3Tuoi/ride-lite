using MatchingPricing.Domain;

namespace MatchingPricing.Domain.Tests;

// Gia co thue va toll giup phat hien sai thu tu hoac ap dung lap he so
public class PricingStrategyTests
{
    [Theory]
    [InlineData(1, 1, 0, 0, 13700, 150700)]
    [InlineData(2, 1, 130000, 0, 26700, 293700)]
    [InlineData(1, 0.8, 0, 27400, 10960, 120560)]
    [InlineData(2, 0.8, 130000, 53400, 21360, 234960)]
    [InlineData(2, 0, 130000, 267000, 0, 0)]
    public void Factory_AllCombinations_RespectFormula(decimal surge, decimal promo,
        decimal surgeAmount, decimal discount, decimal tax, decimal total)
    {
        var context = new PricingContext(10m, 20m, surge, promo);
        IPricingStrategy strategy = new PricingStrategyFactory().Create(context);
        var result = strategy.Calculate(context, new FareParameters(10000m, 1500m, tollFee: 7000m, taxRate: 0.1m));
        Assert.Equal(new FareBreakdown(100000m, 30000m, 130000m, surgeAmount, 7000m, discount, tax, total), result);
        Assert.True(result.TotalFare >= 0m);
    }

    [Fact]
    public void Standard_IgnoresAdjustments_AndKeepsTollAndTax()
    {
        IPricingStrategy strategy = new StandardPricing();
        var result = strategy.Calculate(new PricingContext(10m, 20m, 2m, 0.8m), new FareParameters(10000m, 1500m, 3m, 7000m, 0.5m, 0.1m));
        Assert.Equal(150700m, result.TotalFare);
        Assert.Equal(0m, result.SurgeAmount);
        Assert.Equal(0m, result.PromotionDiscount);
    }

    [Fact]
    public void Decorators_DuplicateOrReordered_DoNotCompoundAdjustments()
    {
        var context = new PricingContext(10m, 20m, 2m, 0.8m);
        IPricingStrategy strategy = new SurgePricing(new PromoPricing(new SurgePricing(new PromoPricing(new StandardPricing()))));
        var result = strategy.Calculate(context, new FareParameters(10000m, 1500m, tollFee: 7000m, taxRate: 0.1m));
        Assert.Equal(234960m, result.TotalFare);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, -1)]
    [InlineData(1, 2)]
    public void Context_InvalidAdjustments_AreRejected(int surge, int promo)
        => Assert.Throws<ArgumentOutOfRangeException>(() => new PricingContext(1m, 1m, surge, promo));
}
