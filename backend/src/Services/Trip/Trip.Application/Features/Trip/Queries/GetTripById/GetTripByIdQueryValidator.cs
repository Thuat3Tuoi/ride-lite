using FluentValidation;
using Trip.Domain.Errors;

namespace Trip.Application.Features.Trip.Queries.GetTripById;

public sealed class GetTripByIdQueryValidator : AbstractValidator<GetTripByIdQuery>
{
    public GetTripByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithState(_ => TripErrors.NotFound);
    }
}

