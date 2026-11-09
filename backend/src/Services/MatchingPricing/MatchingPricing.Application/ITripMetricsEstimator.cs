namespace MatchingPricing.Application;

// Tach nguon uoc luong de sau nay co the thay bang dich vu ban do
public interface ITripMetricsEstimator
{
    TripMetrics Estimate(decimal? distanceKm, decimal? durationMinutes);
}

// Gia tri 0 la du lieu hop le va khac voi du lieu thieu
public sealed record TripMetrics(decimal DistanceKm, decimal DurationMinutes);
