namespace Trip.Application.Features.Rider.Commands.UpdateRiderProfile;

public sealed record UpdateRiderProfileResult(
    long Id,
    string FullName,
    string PhoneNumber,
    DateOnly? DateOfBirth,
    DateTimeOffset UpdatedAt
);

