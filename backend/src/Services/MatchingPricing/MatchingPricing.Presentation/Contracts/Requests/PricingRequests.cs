namespace MatchingPricing.Presentation.Contracts.Requests;

public sealed record EstimatePriceRequest(
    decimal PickupLatitude,
    decimal PickupLongitude,
    decimal DropoffLatitude,
    decimal DropoffLongitude
);

