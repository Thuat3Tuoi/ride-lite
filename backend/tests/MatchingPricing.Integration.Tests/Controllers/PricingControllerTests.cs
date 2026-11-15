using System.Net;
using System.Net.Http.Json;
using MatchingPricing.Integration.Tests.Infrastructure;
using MatchingPricing.Presentation.Contracts.Requests;
using MatchingPricing.Presentation.Contracts.Responses;
using Xunit;

namespace MatchingPricing.Integration.Tests.Controllers;

public sealed class PricingControllerTests
{
    [Fact]
    public async Task EstimatePrice_WithValidCoordinates_Returns200Ok()
    {
        using var factory = new MatchingPricingApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var request = new EstimatePriceRequest(10.7725m, 106.6980m, 10.7900m, 106.7200m);
        var response = await client.PostAsJsonAsync("/api/pricing/estimate", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<EstimatePriceResponse>();
        Assert.NotNull(result);
        Assert.True(result.Fare > 0);
        Assert.True(result.DistanceKm > 0);
        Assert.Equal("VND", result.Currency);
    }

    [Fact]
    public async Task EstimatePrice_WithInvalidCoordinates_Returns400BadRequest()
    {
        using var factory = new MatchingPricingApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var request = new EstimatePriceRequest(999m, 106.6980m, 10.7900m, 106.7200m);
        var response = await client.PostAsJsonAsync("/api/pricing/estimate", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
