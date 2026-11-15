using MatchingPricing.Application.Features.Pricing.Queries.EstimatePrice;
using MatchingPricing.Domain.Errors;
using Xunit;

namespace MatchingPricing.Application.Tests.Features.Pricing;

public sealed class PricingValidatorTests
{
    private readonly EstimatePriceQueryValidator _estimatePriceValidator = new();

    [Fact]
    public void EstimatePrice_WithValidCoordinates_ShouldBeValid()
    {
        var query = new EstimatePriceQuery(10.7725m, 106.6980m, 10.7900m, 106.7200m);
        var result = _estimatePriceValidator.Validate(query);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    public void EstimatePrice_WithInvalidPickupLatitude_ShouldHaveInvalidCoordinatesError(decimal lat)
    {
        var query = new EstimatePriceQuery(lat, 106.6980m, 10.7900m, 106.7200m);
        var result = _estimatePriceValidator.Validate(query);
        Assert.False(result.IsValid);
        Assert.Equal(MatchingPricingErrors.InvalidCoordinates, result.Errors[0].CustomState);
    }

    [Theory]
    [InlineData(-181)]
    [InlineData(181)]
    public void EstimatePrice_WithInvalidPickupLongitude_ShouldHaveInvalidCoordinatesError(decimal lon)
    {
        var query = new EstimatePriceQuery(10.7725m, lon, 10.7900m, 106.7200m);
        var result = _estimatePriceValidator.Validate(query);
        Assert.False(result.IsValid);
        Assert.Equal(MatchingPricingErrors.InvalidCoordinates, result.Errors[0].CustomState);
    }

    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    public void EstimatePrice_WithInvalidDropoffLatitude_ShouldHaveInvalidCoordinatesError(decimal lat)
    {
        var query = new EstimatePriceQuery(10.7725m, 106.6980m, lat, 106.7200m);
        var result = _estimatePriceValidator.Validate(query);
        Assert.False(result.IsValid);
        Assert.Equal(MatchingPricingErrors.InvalidCoordinates, result.Errors[0].CustomState);
    }

    [Theory]
    [InlineData(-181)]
    [InlineData(181)]
    public void EstimatePrice_WithInvalidDropoffLongitude_ShouldHaveInvalidCoordinatesError(decimal lon)
    {
        var query = new EstimatePriceQuery(10.7725m, 106.6980m, 10.7900m, lon);
        var result = _estimatePriceValidator.Validate(query);
        Assert.False(result.IsValid);
        Assert.Equal(MatchingPricingErrors.InvalidCoordinates, result.Errors[0].CustomState);
    }
}

