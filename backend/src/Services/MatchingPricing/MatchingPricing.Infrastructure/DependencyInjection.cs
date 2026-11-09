using MatchingPricing.Application.Abstractions.Pricing;
using MatchingPricing.Application.Configuration;
using MatchingPricing.Application.Features.Quote.Queries.GetQuote;
using MatchingPricing.Application.Services.Pricing;
using MatchingPricing.Domain.Factories;
using MatchingPricing.Infrastructure.Services.Pricing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MatchingPricing.Infrastructure;

// Dang ky cac phu thuoc cua module tai mot noi theo mau Infrastructure tren main
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Van kiem tra cau hinh khi khoi dong sau khi tach khoi Program
        services.AddSingleton<IValidateOptions<PricingOptions>, PricingOptionsValidator>();
        services.AddOptions<PricingOptions>()
            .Bind(configuration.GetSection(PricingOptions.SectionName))
            .ValidateOnStart();

        services.AddSingleton<PricingStrategyFactory>();
        services.AddSingleton<FareRoundingPolicy>();
        services.AddScoped<ITripMetricsEstimator, ConfiguredTripMetricsEstimator>();
        services.AddScoped<GetQuoteHandler>();
        return services;
    }
}
