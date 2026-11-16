using System.Text.Json.Serialization;

namespace MatchingPricing.Domain.Enumerators;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PricingMethod
{
    Regular,
    Surge,
    Discounted
}
