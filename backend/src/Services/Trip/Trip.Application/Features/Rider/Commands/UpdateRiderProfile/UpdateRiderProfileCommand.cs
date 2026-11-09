namespace Trip.Application.Features.Rider.Commands.UpdateRiderProfile;

public sealed record UpdateRiderProfileCommand(
    long Id,
    string FullName,
    string PhoneNumber,
    DateOnly? DateOfBirth = null
);

