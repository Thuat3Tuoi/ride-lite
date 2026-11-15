using FluentValidation;
using Trip.Domain.Errors;

namespace Trip.Application.Features.Trip.Commands.CreateTrip;

public sealed class CreateTripCommandValidator : AbstractValidator<CreateTripCommand>
{
    public CreateTripCommandValidator()
    {
        RuleFor(x => x.RiderId)
            .GreaterThan(0)
            .WithState(_ => TripErrors.RiderNotFound);

        RuleFor(x => x.ClientFare)
            .Must(fare => !fare.HasValue || fare.Value >= 0)
            .WithState(_ => TripErrors.InvalidCost);

        RuleFor(x => x.PickupLatitude)
            .InclusiveBetween(-90m, 90m)
            .WithState(_ => TripErrors.InvalidCoordinates);

        RuleFor(x => x.DropoffLatitude)
            .InclusiveBetween(-90m, 90m)
            .WithState(_ => TripErrors.InvalidCoordinates);

        RuleFor(x => x.PickupLongitude)
            .InclusiveBetween(-180m, 180m)
            .WithState(_ => TripErrors.InvalidCoordinates);

        RuleFor(x => x.DropoffLongitude)
            .InclusiveBetween(-180m, 180m)
            .WithState(_ => TripErrors.InvalidCoordinates);

        RuleFor(x => x.PickupAddress)
            .Cascade(CascadeMode.Stop)
            .Must(a => !string.IsNullOrWhiteSpace(a))
            .WithState(_ => TripErrors.InvalidCoordinates)
            .Must(a => a.Trim().Length <= 500)
            .WithState(_ => TripErrors.InvalidCoordinates);

        RuleFor(x => x.DropoffAddress)
            .Cascade(CascadeMode.Stop)
            .Must(a => !string.IsNullOrWhiteSpace(a))
            .WithState(_ => TripErrors.InvalidCoordinates)
            .Must(a => a.Trim().Length <= 500)
            .WithState(_ => TripErrors.InvalidCoordinates);
    }
}

