namespace Trip.Application.Features.Rider.Commands.CreateRider;

public sealed record CreateRiderCommand(
    string FullName,
    string PhoneNumber,
    DateOnly? DateOfBirth = null
);

