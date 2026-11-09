using MatchingPricing.Domain;
using Microsoft.Extensions.Options;

namespace MatchingPricing.Application;

// Dieu phoi validation, uoc luong va dinh gia, Controller chi tiep nhan HTTP
public sealed class QuoteService(IOptions<PricingOptions> options, ITripMetricsEstimator estimator,
    PricingStrategyFactory strategyFactory, FareRoundingPolicy rounding) : IQuoteService
{
    public QuoteResponse Quote(QuoteRequest request)
    {
        if (request.DistanceKm < 0m || request.DurationMinutes < 0m)
            throw new QuoteValidationException("DistanceKm and DurationMinutes must be non-negative");

        var settings = options.Value;
        if (string.IsNullOrWhiteSpace(request.PickupZone) || !settings.SurgeZones.TryGetValue(request.PickupZone, out var surge))
            throw new QuoteValidationException("PickupZone is not supported");
        decimal promotion = 1m;
        if (request.PromotionCode is not null && !settings.Promotions.TryGetValue(request.PromotionCode, out promotion))
            throw new QuoteValidationException("PromotionCode is not valid");

        try
        {
            // Khong goi estimator khi ca hai gia tri da co, ke ca khi bang 0
            var metrics = request.DistanceKm.HasValue && request.DurationMinutes.HasValue
                ? new TripMetrics(request.DistanceKm.Value, request.DurationMinutes.Value)
                : estimator.Estimate(request.DistanceKm, request.DurationMinutes);
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
            // So qua lon la loi request, khong tra gia bi tran hoac lo thong tin noi bo
            throw new QuoteValidationException("Trip metrics exceed the supported pricing range");
        }
    }
}
