using FluentValidation;

namespace Trip.Application.Features.Trip.Queries.GetTripsByStatus;

public sealed class GetTripsByStatusQueryValidator : AbstractValidator<GetTripsByStatusQuery>
{
    public GetTripsByStatusQueryValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
    }
}

