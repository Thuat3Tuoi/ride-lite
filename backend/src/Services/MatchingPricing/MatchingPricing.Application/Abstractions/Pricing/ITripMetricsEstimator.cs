using MatchingPricing.Application.Models.Pricing;

namespace MatchingPricing.Application.Abstractions.Pricing;

// Tach nguon uoc luong de sau nay co the thay bang dich vu ban do
public interface ITripMetricsEstimator
{
    TripMetrics Estimate(decimal? distanceKm, decimal? durationMinutes);
}
