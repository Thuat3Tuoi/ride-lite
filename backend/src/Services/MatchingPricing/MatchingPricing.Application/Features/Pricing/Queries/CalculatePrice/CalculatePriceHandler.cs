using MatchingPricing.Application.Abstractions.Messaging;
using MatchingPricing.Application.Abstractions.Pricing;

namespace MatchingPricing.Application.Features.Pricing.Queries.CalculatePrice;

public sealed class CalculatePriceHandler : IQueryHandler<CalculatePriceQuery, CalculatePriceResult>
{
    private readonly IPricingService _pricingService;

    public CalculatePriceHandler(IPricingService pricingService)
    {
        _pricingService = pricingService;
    }

    public Task<CalculatePriceResult> HandleAsync(
        CalculatePriceQuery query,
        CancellationToken cancellationToken = default)
    {
        var price = _pricingService.CalculateTripPrice(
            query.CostPerDistance,
            query.Distance,
            query.CostPerTime,
            query.EstimatedTime,
            query.AdditionalFee,
            query.TaxRate,
            query.PricingMethod
        );

        return Task.FromResult(new CalculatePriceResult(price, query.PricingMethod));
    }
}

