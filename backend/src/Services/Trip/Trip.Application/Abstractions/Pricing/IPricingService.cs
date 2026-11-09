namespace Trip.Application.Abstractions.Pricing;

public interface IPricingService
{
    Task<decimal> CalculateFareAsync(
        decimal pickupLatitude,
        decimal pickupLongitude,
        decimal dropoffLatitude,
        decimal dropoffLongitude,
        CancellationToken cancellationToken = default);
}

