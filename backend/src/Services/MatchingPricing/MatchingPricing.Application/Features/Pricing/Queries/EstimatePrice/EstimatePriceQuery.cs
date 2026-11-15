namespace MatchingPricing.Application.Features.Pricing.Queries.EstimatePrice;

public sealed record EstimatePriceQuery(
    decimal PickupLatitude,
    decimal PickupLongitude,
    decimal DropoffLatitude,
    decimal DropoffLongitude
);
