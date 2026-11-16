using MatchingPricing.Domain.Enumerators;

namespace MatchingPricing.Application.Features.Pricing.Queries.CalculatePrice;

public sealed record CalculatePriceQuery(
    decimal CostPerDistance,
    decimal Distance,
    decimal CostPerTime,
    decimal EstimatedTime,
    decimal AdditionalFee,
    decimal TaxRate,
    PricingMethod PricingMethod
);
