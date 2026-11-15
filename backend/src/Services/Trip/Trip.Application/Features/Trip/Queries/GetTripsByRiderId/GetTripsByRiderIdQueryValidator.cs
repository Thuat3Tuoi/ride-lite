using FluentValidation;
using Trip.Domain.Errors;

namespace Trip.Application.Features.Trip.Queries.GetTripsByRiderId;

public sealed class GetTripsByRiderIdQueryValidator : AbstractValidator<GetTripsByRiderIdQuery>
{
    public GetTripsByRiderIdQueryValidator()
    {
        RuleFor(x => x.RiderId)
            .GreaterThan(0)
            .WithState(_ => TripErrors.RiderNotFound);
    }
}

