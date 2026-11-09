using MatchingPricing.Application.Abstractions.Pricing;
using MatchingPricing.Application.Configuration;
using MatchingPricing.Application.Exceptions;
using MatchingPricing.Application.Models.Pricing;
using Microsoft.Extensions.Options;

namespace MatchingPricing.Infrastructure.Services.Pricing;

// Chi suy ra thoi gian tu quang duong, khong suy doan quang duong tu thoi gian
public sealed class ConfiguredTripMetricsEstimator(IOptions<PricingOptions> options) : ITripMetricsEstimator
{
    public TripMetrics Estimate(decimal? distanceKm, decimal? durationMinutes)
    {
        if (!distanceKm.HasValue)
            throw new QuoteValidationException("DistanceKm is required because no route data is available to estimate distance");
        if (distanceKm < 0m || durationMinutes < 0m)
            throw new QuoteValidationException("DistanceKm and DurationMinutes must be non-negative");

        // Doi sang phut truoc khi chia de tranh sai so cua phep chia lap vo han
        decimal duration = durationMinutes ?? distanceKm.Value * 60m / options.Value.AverageSpeedKmPerHour;
        return new(distanceKm.Value, duration);
    }
}
