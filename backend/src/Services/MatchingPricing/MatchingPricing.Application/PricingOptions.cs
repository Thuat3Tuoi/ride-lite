namespace MatchingPricing.Application;

// Du lieu gia nam trong section Pricing, gia am mac dinh buoc phai cau hinh don gia
public sealed class PricingOptions
{
    public const string SectionName = "Pricing";
    public decimal DistanceRate { get; set; } = -1m;
    public decimal TimeRate { get; set; } = -1m;
    public decimal TollFee { get; set; }
    public decimal TaxRate { get; set; }
    public decimal AverageSpeedKmPerHour { get; set; }
    public Dictionary<string, decimal> SurgeZones { get; set; } = new();
    public Dictionary<string, decimal> Promotions { get; set; } = new();
}
