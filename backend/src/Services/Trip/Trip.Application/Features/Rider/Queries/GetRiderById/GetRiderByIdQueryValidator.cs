using FluentValidation;
using Trip.Domain.Errors;

namespace Trip.Application.Features.Rider.Queries.GetRiderById;

public sealed class GetRiderByIdQueryValidator : AbstractValidator<GetRiderByIdQuery>
{
    public GetRiderByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithState(_ => TripErrors.RiderNotFound);
    }
}

