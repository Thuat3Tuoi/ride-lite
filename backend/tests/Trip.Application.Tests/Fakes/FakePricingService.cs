using Trip.Application.Abstractions.Pricing;

namespace Trip.Application.Tests.Fakes;

public sealed class FakePricingService : IPricingService
{
    public decimal FixedFare { get; set; } = 50000m;
    public bool WasCalled { get; private set; }

    public Task<decimal> CalculateFareAsync(
        decimal pickupLatitude,
        decimal pickupLongitude,
        decimal dropoffLatitude,
        decimal dropoffLongitude,
        CancellationToken cancellationToken = default)
    {
        WasCalled = true;
        return Task.FromResult(FixedFare);
    }
}

