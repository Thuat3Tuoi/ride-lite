using Trip.Application.Abstractions.Validation;
using Trip.Application.Exceptions;

namespace Trip.Application.Trips.Queries.GetTripById;

public sealed class GetTripByIdValidator : IValidator<GetTripByIdQuery>
{
    public void Validate(GetTripByIdQuery query)
    {
        if (query.TripId <= 0)
        {
            throw new TripValidationException("Trip id must be positive.");
        }
    }
}
