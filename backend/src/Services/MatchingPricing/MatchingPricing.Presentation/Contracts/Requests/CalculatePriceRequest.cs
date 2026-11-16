using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MatchingPricing.Domain.Enumerators;

namespace MatchingPricing.Presentation.Contracts.Requests;

public sealed record CalculatePriceRequest(
    [Range(0, double.MaxValue, ErrorMessage = "Cost per distance must be non-negative.")]
    decimal CostPerDistance,

    [Range(0, double.MaxValue, ErrorMessage = "Distance must be non-negative.")]
    decimal Distance,

    [Range(0, double.MaxValue, ErrorMessage = "Cost per time must be non-negative.")]
    decimal CostPerTime,

    [Range(0, double.MaxValue, ErrorMessage = "Estimated time must be non-negative.")]
    decimal EstimatedTime,

    [Range(0, double.MaxValue, ErrorMessage = "Additional fee must be non-negative.")]
    decimal AdditionalFee,

    [Range(0, double.MaxValue, ErrorMessage = "Tax rate must be non-negative.")]
    decimal TaxRate,

    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    [EnumDataType(typeof(PricingMethod), ErrorMessage = "Invalid pricing method.")]
    PricingMethod PricingMethod
);
