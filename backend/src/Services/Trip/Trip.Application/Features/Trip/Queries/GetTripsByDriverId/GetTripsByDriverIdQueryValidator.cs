using FluentValidation;
using Trip.Domain.Errors;

namespace Trip.Application.Features.Trip.Queries.GetTripsByDriverId;

public sealed class GetTripsByDriverIdQueryValidator : AbstractValidator<GetTripsByDriverIdQuery>
{
    public GetTripsByDriverIdQueryValidator()
    {
        RuleFor(x => x.DriverId)
            .GreaterThan(0)
            .WithState(_ => TripErrors.InvalidDriverId);
    }
}

