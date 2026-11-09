using MatchingPricing.Domain.Services;
using MatchingPricing.Domain.ValueObjects;

namespace MatchingPricing.Domain.Strategies;

// Ghep chinh sach truoc, chi tinh cong thuc mot lan de tranh cong lap va sai thu tu
public interface IPricingStrategy
{
    FareParameters GetParameters(PricingContext context, FareParameters parameters);

    FareBreakdown Calculate(PricingContext context, FareParameters parameters)
        => FareFormula.Calculate(context, GetParameters(context, parameters));
}
