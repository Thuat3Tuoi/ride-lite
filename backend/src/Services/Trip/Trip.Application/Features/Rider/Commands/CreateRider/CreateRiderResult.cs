namespace Trip.Application.Features.Rider.Commands.CreateRider;

public sealed record CreateRiderResult(
    long Id,
    string FullName,
    string PhoneNumber,
    DateOnly? DateOfBirth,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);

