using MatchingPricing.Domain.Enumerators;

namespace MatchingPricing.Application.Features.Pricing.Queries.CalculatePrice;

public sealed record CalculatePriceResult(
    decimal Price,
    PricingMethod PricingMethod
);

