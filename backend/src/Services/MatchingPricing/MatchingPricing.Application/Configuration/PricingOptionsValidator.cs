using Microsoft.Extensions.Options;

namespace MatchingPricing.Application.Configuration;

// Chan cau hinh sai ngay khi khoi dong thay vi de request dau tien moi phat hien
public sealed class PricingOptionsValidator : IValidateOptions<PricingOptions>
{
    public ValidateOptionsResult Validate(string? name, PricingOptions options)
    {
        if (options.DistanceRate < 0m || options.TimeRate < 0m || options.TollFee < 0m || options.TaxRate < 0m)
            return ValidateOptionsResult.Fail("Pricing rates, toll and tax must be non-negative");
        if (options.AverageSpeedKmPerHour <= 0m)
            return ValidateOptionsResult.Fail("Pricing:AverageSpeedKmPerHour must be positive");
        if (options.SurgeZones is null || !options.SurgeZones.ContainsKey("default") ||
            options.SurgeZones.Any(pair => string.IsNullOrWhiteSpace(pair.Key) || pair.Value < 1m))
            return ValidateOptionsResult.Fail("Pricing:SurgeZones must include default and multipliers >= 1");
        if (options.Promotions is null || options.Promotions.Any(pair =>
            string.IsNullOrWhiteSpace(pair.Key) || pair.Value < 0m || pair.Value > 1m))
            return ValidateOptionsResult.Fail("Pricing:Promotions factors must be between 0 and 1");
        return ValidateOptionsResult.Success;
    }
}
