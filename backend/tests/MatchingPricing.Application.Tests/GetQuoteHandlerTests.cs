using MatchingPricing.Application.Abstractions.Pricing;
using MatchingPricing.Application.Configuration;
using MatchingPricing.Application.Exceptions;
using MatchingPricing.Application.Features.Quote.Queries.GetQuote;
using MatchingPricing.Application.Services.Pricing;
using MatchingPricing.Domain.Factories;
using MatchingPricing.Infrastructure.Services.Pricing;
using Microsoft.Extensions.Options;
using Moq;

namespace MatchingPricing.Application.Tests;

// Mock chi xac nhan viec uoc luong khi du lieu bi thieu
public class GetQuoteHandlerTests
{
    private static PricingOptions Settings() => new()
    {
        DistanceRate = 10000m, TimeRate = 1500m, TollFee = 7000m,
        AverageSpeedKmPerHour = 30m,
        SurgeZones = new() { ["default"] = 1m, ["busy-demo"] = 2m },
        Promotions = new() { ["DEMO20"] = 0.8m }
    };

    private static GetQuoteHandler Handler(PricingOptions? settings = null, ITripMetricsEstimator? estimator = null)
    {
        var options = Options.Create(settings ?? Settings());
        return new(options, estimator ?? new ConfiguredTripMetricsEstimator(options), new PricingStrategyFactory(), new FareRoundingPolicy());
    }

    [Fact]
    public void CompleteMetrics_DoNotCallEstimator_AndReturn137000()
    {
        var estimator = new Mock<ITripMetricsEstimator>(MockBehavior.Strict);
        var result = Handler(estimator: estimator.Object).Handle(new(10m, 20m));
        Assert.Equal(137000m, result.TotalFare);
        Assert.Equal(10m, result.DistanceKm);
        Assert.Equal(20m, result.DurationMinutes);
        estimator.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("default", null, 137000)]
    [InlineData("busy-demo", null, 267000)]
    [InlineData("default", "DEMO20", 109600)]
    [InlineData("busy-demo", "DEMO20", 213600)]
    public void Policies_AreResolvedFromServerConfiguration(string zone, string? code, int total)
    {
        var result = Handler().Handle(new(10m, 20m, zone, code));
        Assert.Equal(total, result.TotalFare);
    }

    [Fact]
    public void MissingDuration_IsEstimatedFromConfiguredSpeed()
    {
        var result = Handler().Handle(new(10m, null));
        Assert.Equal(20m, result.DurationMinutes);
        Assert.Equal(137000m, result.TotalFare);
    }

    [Theory]
    [InlineData(null, 20)]
    [InlineData(null, null)]
    [InlineData(-1, 20)]
    [InlineData(10, -1)]
    public void InvalidOrInsufficientMetrics_AreRejected(int? distance, int? duration)
        => Assert.Throws<QuoteValidationException>(() => Handler().Handle(new(distance, duration)));

    [Theory]
    [InlineData(0, 20, 37000)]
    [InlineData(10, 0, 107000)]
    [InlineData(0, 0, 7000)]
    public void ZeroMetrics_ArePreserved(int distance, int duration, int total)
        => Assert.Equal(total, Handler().Handle(new(distance, duration)).TotalFare);

    [Fact]
    public void ZeroDistance_WithMissingDuration_EstimatesZero()
        => Assert.Equal(0m, Handler().Handle(new(0m, null)).DurationMinutes);

    [Theory]
    [InlineData("unknown", null)]
    [InlineData("default", "INVALID")]
    [InlineData("", null)]
    public void UnknownPolicies_AreRejected(string zone, string? code)
        => Assert.Throws<QuoteValidationException>(() => Handler().Handle(new(10m, 20m, zone, code)));

    [Theory]
    [InlineData("100.4999", 100)]
    [InlineData("100.5", 101)]
    [InlineData("100.5001", 101)]
    public void Finalization_RoundsOnce_AndExposesReconciliation(string rate, int expected)
    {
        var settings = Settings();
        settings.DistanceRate = decimal.Parse(rate, System.Globalization.CultureInfo.InvariantCulture);
        settings.TimeRate = 0m;
        settings.TollFee = 0m;
        var result = Handler(settings).Handle(new(1m, 0m));
        Assert.Equal(expected, result.TotalFare);
        Assert.Equal(settings.DistanceRate, result.DistanceFare);
        Assert.Equal(result.TotalFare, result.Subtotal + result.SurgeAmount + result.TollFee
            - result.PromotionDiscount + result.TaxAmount + result.RoundingAdjustment);
    }

    [Fact]
    public void ExtremeMetrics_ReturnValidationInsteadOfOverflow()
        => Assert.Throws<QuoteValidationException>(() => Handler().Handle(new(decimal.MaxValue, 0m)));

    [Fact]
    public void InvalidServerConfiguration_IsRejected()
    {
        var invalid = new List<PricingOptions>();
        var settings = Settings(); settings.DistanceRate = -1m; invalid.Add(settings);
        settings = Settings(); settings.TimeRate = -1m; invalid.Add(settings);
        settings = Settings(); settings.TollFee = -1m; invalid.Add(settings);
        settings = Settings(); settings.TaxRate = -1m; invalid.Add(settings);
        settings = Settings(); settings.AverageSpeedKmPerHour = 0m; invalid.Add(settings);
        settings = Settings(); settings.SurgeZones["default"] = 0.5m; invalid.Add(settings);
        settings = Settings(); settings.Promotions["bad"] = 1.1m; invalid.Add(settings);
        settings = Settings(); settings.Promotions["bad"] = -0.1m; invalid.Add(settings);
        settings = Settings(); settings.SurgeZones.Clear(); invalid.Add(settings);
        foreach (var value in invalid)
            Assert.True(new PricingOptionsValidator().Validate(null, value).Failed);
        Assert.True(new PricingOptionsValidator().Validate(null, Settings()).Succeeded);
    }
}
