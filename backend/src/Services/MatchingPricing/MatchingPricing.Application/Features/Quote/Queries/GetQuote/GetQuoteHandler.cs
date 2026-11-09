using MatchingPricing.Application.Abstractions.Pricing;
using MatchingPricing.Application.Configuration;
using MatchingPricing.Application.Exceptions;
using MatchingPricing.Application.Models.Pricing;
using MatchingPricing.Application.Services.Pricing;
using MatchingPricing.Domain.Factories;
using MatchingPricing.Domain.ValueObjects;
using Microsoft.Extensions.Options;

namespace MatchingPricing.Application.Features.Quote.Queries.GetQuote;

// Dieu phoi validation, uoc luong va dinh gia, Controller chi tiep nhan HTTP
public sealed class GetQuoteHandler(IOptions<PricingOptions> options, ITripMetricsEstimator estimator,
    PricingStrategyFactory strategyFactory, FareRoundingPolicy rounding)
{
    public GetQuoteResult Handle(GetQuoteQuery query)
    {
        if (query.DistanceKm < 0m || query.DurationMinutes < 0m)
            throw new QuoteValidationException("DistanceKm and DurationMinutes must be non-negative");

        var settings = options.Value;
        if (string.IsNullOrWhiteSpace(query.PickupZone) || !settings.SurgeZones.TryGetValue(query.PickupZone, out var surge))
            throw new QuoteValidationException("PickupZone is not supported");
        decimal promotion = 1m;
        if (query.PromotionCode is not null && !settings.Promotions.TryGetValue(query.PromotionCode, out promotion))
            throw new QuoteValidationException("PromotionCode is not valid");

        try
        {
            // Khong goi estimator khi ca hai gia tri da co, ke ca khi bang 0
            var metrics = query.DistanceKm.HasValue && query.DurationMinutes.HasValue
                ? new TripMetrics(query.DistanceKm.Value, query.DurationMinutes.Value)
                : estimator.Estimate(query.DistanceKm, query.DurationMinutes);
            var context = new PricingContext(metrics.DistanceKm, metrics.DurationMinutes, surge, promotion);
            var parameters = new FareParameters(settings.DistanceRate, settings.TimeRate,
                tollFee: settings.TollFee, taxRate: settings.TaxRate);
            var fare = strategyFactory.Create(context).Calculate(context, parameters);
            decimal total = rounding.Round(fare.TotalFare);
            return new(metrics.DistanceKm, metrics.DurationMinutes, fare.DistanceFare, fare.TimeFare,
                fare.Subtotal, fare.SurgeAmount, fare.TollFee, fare.PromotionDiscount, fare.TaxAmount,
                total - fare.TotalFare, total, fare.Currency);
        }
        catch (OverflowException)
        {
            // So qua lon la loi du lieu dau vao, khong tra gia bi tran hoac lo thong tin noi bo
            throw new QuoteValidationException("Trip metrics exceed the supported pricing range");
        }
    }
}
