using System.Text.RegularExpressions;
using FluentValidation;
using Trip.Domain.Errors;

namespace Trip.Application.Features.Rider.Commands.UpdateRiderProfile;

public sealed partial class UpdateRiderProfileCommandValidator : AbstractValidator<UpdateRiderProfileCommand>
{
    private static readonly Regex PhoneRegex = new(@"^0\d{9}$", RegexOptions.Compiled);

    public UpdateRiderProfileCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithState(_ => TripErrors.RiderNotFound);

        RuleFor(x => x.FullName)
            .Cascade(CascadeMode.Stop)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithState(_ => TripErrors.InvalidFullName)
            .Must(name => name.Trim().Length <= 255)
            .WithState(_ => TripErrors.InvalidFullName);

        RuleFor(x => x.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .Must(phone => !string.IsNullOrWhiteSpace(phone))
            .WithState(_ => TripErrors.InvalidPhoneNumber)
            .Must(phone => PhoneRegex.IsMatch(phone.Trim()))
            .WithState(_ => TripErrors.InvalidPhoneNumber);

        RuleFor(x => x.DateOfBirth)
            .Must(dob => !dob.HasValue || dob.Value <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithState(_ => TripErrors.InvalidDateOfBirth);
    }
}

