using Trip.Application.Abstractions.Pricing;

namespace Trip.Infrastructure.Services.Pricing;

public sealed class PricingService : IPricingService
{
    private const decimal BaseFare = 15000m;
    private const decimal BaseDistanceKm = 2.0m;
    private const decimal RatePerKm = 10000m;

    public Task<decimal> CalculateFareAsync(
        decimal pickupLatitude,
        decimal pickupLongitude,
        decimal dropoffLatitude,
        decimal dropoffLongitude,
        CancellationToken cancellationToken = default)
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

        // Persist whole VND integer amounts for decimal(14,0)
        var roundedFare = Math.Round(fare, 0, MidpointRounding.AwayFromZero);

        return Task.FromResult(roundedFare);
    }

    private static double CalculateHaversineDistance(
        double lat1, double lon1,
        double lat2, double lon2)
    {
        const double EarthRadiusKm = 6371.0;

        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c;
    }

    private static double ToRadians(double degrees) => degrees * (Math.PI / 180.0);
}

