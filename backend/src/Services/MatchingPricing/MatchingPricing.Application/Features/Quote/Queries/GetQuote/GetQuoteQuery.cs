namespace MatchingPricing.Application.Features.Quote.Queries.GetQuote;

// Du lieu truy van bao gia doc lap voi contract HTTP
public sealed record GetQuoteQuery(decimal? DistanceKm = null, decimal? DurationMinutes = null,
    string PickupZone = "default", string? PromotionCode = null);
