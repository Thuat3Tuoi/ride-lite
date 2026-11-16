using MatchingPricing.Application.Abstractions.Pricing;
using MatchingPricing.Domain.Enumerators;

namespace MatchingPricing.Infrastructure.Services.Pricing;

public sealed class PricingCalculator : IPricingCalculator
{
    private readonly IPricingService _pricingService;

    public PricingCalculator(IPricingService pricingService)
    {
        _pricingService = pricingService;
    }

    public decimal CalculateFare(
        decimal pickupLatitude,
        decimal pickupLongitude,
        decimal dropoffLatitude,
        decimal dropoffLongitude)
    {
        var distanceKm = (decimal)CalculateHaversineDistance(
            (double)pickupLatitude,
            (double)pickupLongitude,
            (double)dropoffLatitude,
            (double)dropoffLongitude
        );

        var estimatedTimeMinutes = Math.Max(5m, Math.Round(distanceKm * 2.5m, 1));

        return _pricingService.CalculateTripPrice(
            costPerDistance: 10000m,
            distance: distanceKm,
            costPerTime: 500m,
            estimatedTime: estimatedTimeMinutes,
            additionalFee: 0m,
            taxRate: 0.08m,
            pricingMethod: PricingMethod.Regular
        );
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

