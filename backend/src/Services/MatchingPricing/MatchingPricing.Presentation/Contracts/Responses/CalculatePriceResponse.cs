using System.Text.Json.Serialization;
using MatchingPricing.Domain.Enumerators;

namespace MatchingPricing.Presentation.Contracts.Responses;

public sealed record CalculatePriceResponse(
    decimal Price,
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    PricingMethod PricingMethod
);
