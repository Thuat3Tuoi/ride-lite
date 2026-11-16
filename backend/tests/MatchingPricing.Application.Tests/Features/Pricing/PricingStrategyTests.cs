using MatchingPricing.Application.Abstractions.Pricing;
using MatchingPricing.Application.Features.Pricing.Factory;
using MatchingPricing.Application.Features.Pricing.Queries.CalculatePrice;
using MatchingPricing.Application.Features.Pricing.Services;
using MatchingPricing.Application.Features.Pricing.Strategies;
using MatchingPricing.Domain.Enumerators;
using Xunit;

namespace MatchingPricing.Application.Tests.Features.Pricing;

public class PricingStrategyTests
{
    private readonly IPricingStrategy[] _strategies =
    [
        new CalculateRegularPriceStrategy(),
        new CalculateSurgePriceStrategy(),
        new CalculateDiscountedPriceStrategy()
    ];

    [Fact]
    public void Factory_ShouldResolveAllRegisteredStrategies()
    {
        var factory = new PricingStrategyFactory(_strategies);

        var regularStrategy = factory.GetPricingStrategy(PricingMethod.Regular);
        var surgeStrategy = factory.GetPricingStrategy(PricingMethod.Surge);
        var discountedStrategy = factory.GetPricingStrategy(PricingMethod.Discounted);

        Assert.IsType<CalculateRegularPriceStrategy>(regularStrategy);
        Assert.IsType<CalculateSurgePriceStrategy>(surgeStrategy);
        Assert.IsType<CalculateDiscountedPriceStrategy>(discountedStrategy);
    }

    [Fact]
    public void Factory_UnknownPricingMethod_ShouldThrowKeyNotFoundException()
    {
        var factory = new PricingStrategyFactory(_strategies);

        Assert.Throws<KeyNotFoundException>(() => factory.GetPricingStrategy((PricingMethod)999));
    }

    [Fact]
    public void RegularStrategy_ShouldCalculateStandardPrice()
    {
        var strategy = new CalculateRegularPriceStrategy();

        // ((10000 * 5 + 1000 * 10) * 1.0 + 0) * 1.0 * (1 + 0.1) = 66000
        var price = strategy.CalculateTripFare(
            costPerDistance: 10000m,
            distance: 5m,
            costPerTime: 1000m,
            estimatedTime: 10m,
            additionalFee: 0m,
            taxRate: 0.10m
        );

        Assert.Equal(66000m, price);
    }

    [Fact]
    public void SurgeStrategy_ShouldApplySurgeMultiplier()
    {
        var strategy = new CalculateSurgePriceStrategy();

        // ((10000 * 5 + 1000 * 10) * 1.5 + 0) * 1.0 * (1 + 0.1) = 90000 * 1.1 = 99000
        var price = strategy.CalculateTripFare(
            costPerDistance: 10000m,
            distance: 5m,
            costPerTime: 1000m,
            estimatedTime: 10m,
            additionalFee: 0m,
            taxRate: 0.10m
        );

        Assert.Equal(99000m, price);
    }

    [Fact]
    public void DiscountedStrategy_ShouldApplyDiscountFactor()
    {
        var strategy = new CalculateDiscountedPriceStrategy();

        // ((10000 * 5 + 1000 * 10) * 1.0 + 0) * 0.8 * (1 + 0.1) = 60000 * 0.8 * 1.1 = 52800
        var price = strategy.CalculateTripFare(
            costPerDistance: 10000m,
            distance: 5m,
            costPerTime: 1000m,
            estimatedTime: 10m,
            additionalFee: 0m,
            taxRate: 0.10m
        );

        Assert.Equal(52800m, price);
    }

    [Fact]
    public async Task CalculatePriceHandler_ShouldReturnCorrectCalculatedResult()
    {
        var factory = new PricingStrategyFactory(_strategies);
        var service = new PricingService(factory);
        var handler = new CalculatePriceHandler(service);

        var query = new CalculatePriceQuery(
            CostPerDistance: 10000m,
            Distance: 5m,
            CostPerTime: 1000m,
            EstimatedTime: 10m,
            AdditionalFee: 5000m,
            TaxRate: 0.10m,
            PricingMethod: PricingMethod.Regular
        );

        var result = await handler.HandleAsync(query);

        Assert.NotNull(result);
        Assert.Equal(71500m, result.Price);
        Assert.Equal(PricingMethod.Regular, result.PricingMethod);
    }
}

