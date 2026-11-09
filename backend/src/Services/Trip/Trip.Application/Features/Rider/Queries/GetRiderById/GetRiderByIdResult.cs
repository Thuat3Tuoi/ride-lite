namespace Trip.Application.Features.Rider.Queries.GetRiderById;

public sealed record GetRiderByIdResult(
    long Id,
    string FullName,
    string PhoneNumber,
    DateOnly? DateOfBirth,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);

