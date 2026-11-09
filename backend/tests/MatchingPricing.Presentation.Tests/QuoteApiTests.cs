using System.Net.Http.Json;
using System.Net;
using System.Text.Json;
using System.Text;
using MatchingPricing.Application.Abstractions.Pricing;
using MatchingPricing.Application.Configuration;
using MatchingPricing.Presentation.Contracts.Responses;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace MatchingPricing.Presentation.Tests;

// Kiem tra qua pipeline HTTP that, gom model binding va xu ly loi an stack trace
public class QuoteApiTests
{
    [Theory]
    [InlineData("/matching-pricing/quote", "{\"distanceKm\":10,\"durationMinutes\":20}", 137000)]
    [InlineData("/quote", "{\"distanceKm\":10,\"durationMinutes\":20}", 137000)]
    [InlineData("/matching-pricing/quote", "{\"distanceKm\":10,\"durationMinutes\":20,\"pickupZone\":\"busy-demo\"}", 267000)]
    [InlineData("/matching-pricing/quote", "{\"distanceKm\":10,\"durationMinutes\":20,\"promotionCode\":\"DEMO20\"}", 109600)]
    [InlineData("/matching-pricing/quote", "{\"distanceKm\":10,\"durationMinutes\":20,\"pickupZone\":\"busy-demo\",\"promotionCode\":\"DEMO20\"}", 213600)]
    [InlineData("/matching-pricing/quote", "{\"distanceKm\":10}", 137000)]
    [InlineData("/matching-pricing/quote", "{\"distanceKm\":0,\"durationMinutes\":0}", 7000)]
    public async Task Quote_ValidRequests_ReturnReconciledBreakdown(string path, string json, int total)
    {
        await using var factory = new WebApplicationFactory<global::Program>();
        using var client = factory.CreateClient();
        using var response = await client.PostAsync(path, new StringContent(json, Encoding.UTF8, "application/json"), TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var quote = (await response.Content.ReadFromJsonAsync<QuoteResponse>(TestContext.Current.CancellationToken))!;
        Assert.Equal(total, quote.TotalFare);
        Assert.Equal("VND", quote.Currency);
        Assert.Equal(quote.DistanceFare + quote.TimeFare, quote.Subtotal);
        Assert.Equal(quote.TotalFare, quote.Subtotal + quote.SurgeAmount + quote.TollFee
            - quote.PromotionDiscount + quote.TaxAmount + quote.RoundingAdjustment);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"durationMinutes\":20}")]
    [InlineData("{\"distanceKm\":-1,\"durationMinutes\":20}")]
    [InlineData("{\"distanceKm\":10,\"durationMinutes\":-1}")]
    [InlineData("{\"distanceKm\":10,\"distanceRate\":1}")]
    [InlineData("{\"distanceKm\":10,\"promotionFactor\":0}")]
    [InlineData("{\"distanceKm\":10,\"surgeMultiplier\":1}")]
    [InlineData("{\"distanceKm\":10,\"taxRate\":0}")]
    [InlineData("{\"distanceKm\":10,\"tollFee\":0}")]
    [InlineData("{\"distanceKm\":10,\"pickupZone\":\"unknown\"}")]
    [InlineData("{\"distanceKm\":10,\"promotionCode\":\"unknown\"}")]
    [InlineData("{\"distanceKm\":79228162514264337593543950335,\"durationMinutes\":0}")]
    [InlineData("{\"distanceKm\":1e100}")]
    [InlineData("null")]
    [InlineData("{")]
    public async Task Quote_InvalidRequests_Return400(string json)
    {
        await using var factory = new WebApplicationFactory<global::Program>();
        using var client = factory.CreateClient();
        using var response = await client.PostAsync("/matching-pricing/quote", new StringContent(json, Encoding.UTF8, "application/json"), TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [InlineData("/matching-pricing/health")]
    [InlineData("/health")]
    public async Task Health_ReturnsStructuredStatus(string path)
    {
        await using var factory = new WebApplicationFactory<global::Program>();
        using var client = factory.CreateClient();
        using var response = await client.GetAsync(path, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(TestContext.Current.CancellationToken);
        Assert.Equal("Healthy", body.GetProperty("status").GetString());
        Assert.Equal("MatchingPricing", body.GetProperty("module").GetString());
    }

    [Fact]
    public async Task UnexpectedFailure_Returns500_WithoutInternalDetails()
    {
        var estimator = new Mock<ITripMetricsEstimator>();
        estimator.Setup(value => value.Estimate(It.IsAny<decimal?>(), It.IsAny<decimal?>()))
            .Throws(new InvalidOperationException("private-marker"));
        await using var factory = new WebApplicationFactory<global::Program>().WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => services.Replace(ServiceDescriptor.Singleton(estimator.Object))));
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync("/matching-pricing/quote", new { distanceKm = 10 }, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        estimator.Verify(value => value.Estimate(10m, null), Times.Once);
        string body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.DoesNotContain("private-marker", body);
        Assert.DoesNotContain("InvalidOperationException", body);
        Assert.DoesNotContain("stackTrace", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void InvalidConfiguration_PreventsStartup()
    {
        using var factory = new WebApplicationFactory<global::Program>().WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => services.PostConfigure<PricingOptions>(settings => settings.DistanceRate = -1m)));
        Assert.Throws<OptionsValidationException>(() => factory.CreateClient());
    }
}
