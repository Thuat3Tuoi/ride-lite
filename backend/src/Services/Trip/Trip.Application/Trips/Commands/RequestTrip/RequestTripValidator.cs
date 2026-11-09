using Trip.Application.Abstractions.Validation;
using Trip.Application.Exceptions;

namespace Trip.Application.Trips.Commands.RequestTrip;

public sealed class RequestTripValidator : IValidator<RequestTripCommand>
{
    public void Validate(RequestTripCommand command)
    {
        if (command.RiderId <= 0)
        {
            throw new TripValidationException("Rider id must be positive.");
        }

        if (string.IsNullOrWhiteSpace(command.PickupAddress))
        {
            throw new TripValidationException("Pickup address is required.");
        }

        if (string.IsNullOrWhiteSpace(command.DropoffAddress))
        {
            throw new TripValidationException("Dropoff address is required.");
        }

        EnsureCoordinate(command.PickupLatitude, command.PickupLongitude, "pickup");
        EnsureCoordinate(command.DropoffLatitude, command.DropoffLongitude, "dropoff");
    }

    private static void EnsureCoordinate(decimal latitude, decimal longitude, string label)
    {
        if (latitude is < -90 or > 90)
        {
            throw new TripValidationException($"The {label} latitude must be between -90 and 90.");
        }

        if (longitude is < -180 or > 180)
        {
            throw new TripValidationException($"The {label} longitude must be between -180 and 180.");
        }
    }
}
