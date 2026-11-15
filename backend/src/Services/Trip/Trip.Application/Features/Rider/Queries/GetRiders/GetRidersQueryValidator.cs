using FluentValidation;

namespace Trip.Application.Features.Rider.Queries.GetRiders;

public sealed class GetRidersQueryValidator : AbstractValidator<GetRidersQuery>
{
    public GetRidersQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
