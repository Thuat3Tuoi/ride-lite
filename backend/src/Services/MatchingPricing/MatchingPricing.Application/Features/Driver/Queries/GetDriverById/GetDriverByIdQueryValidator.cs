using FluentValidation;
using MatchingPricing.Domain.Errors;

namespace MatchingPricing.Application.Features.Driver.Queries.GetDriverById;

public sealed class GetDriverByIdQueryValidator : AbstractValidator<GetDriverByIdQuery>
{
    public GetDriverByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithState(_ => MatchingPricingErrors.DriverNotFound);
    }
}

