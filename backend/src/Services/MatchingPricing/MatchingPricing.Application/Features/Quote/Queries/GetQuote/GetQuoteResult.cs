namespace MatchingPricing.Application.Features.Quote.Queries.GetQuote;

// RoundingAdjustment giup doi chieu thanh phan chua lam tron voi tong VND nguyen
public sealed record GetQuoteResult(decimal DistanceKm, decimal DurationMinutes,
    decimal DistanceFare, decimal TimeFare, decimal Subtotal, decimal SurgeAmount,
    decimal TollFee, decimal PromotionDiscount, decimal TaxAmount,
    decimal RoundingAdjustment, decimal TotalFare, string Currency);
