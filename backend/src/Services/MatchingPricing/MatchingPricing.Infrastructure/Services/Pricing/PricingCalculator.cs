using MatchingPricing.Application.Abstractions.Pricing;

namespace MatchingPricing.Infrastructure.Services.Pricing;

public sealed class PricingCalculator : IPricingCalculator
{
    private const decimal BaseFare = 15000m;
    private const decimal BaseDistanceKm = 2.0m;
    private const decimal RatePerKm = 10000m;

    public decimal CalculateFare(
        decimal pickupLatitude,
        decimal pickupLongitude,
        decimal dropoffLatitude,
        decimal dropoffLongitude)
    {
        var distanceKm = CalculateHaversineDistance(
            (double)pickupLatitude,
            (double)pickupLongitude,
            (double)dropoffLatitude,
            (double)dropoffLongitude
        );

        decimal fare = BaseFare;
        var decimalDistance = (decimal)distanceKm;

        if (decimalDistance > BaseDistanceKm)
        {
            var extraDistance = decimalDistance - BaseDistanceKm;
            fare += extraDistance * RatePerKm;
        }

        return Math.Round(fare, 0, MidpointRounding.AwayFromZero);
    }

    private static double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double EarthRadiusKm = 6371.0;

        var dLat = (lat2 - lat1) * Math.PI / 180.0;
        var dLon = (lon2 - lon1) * Math.PI / 180.0;

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c;
    }
}

