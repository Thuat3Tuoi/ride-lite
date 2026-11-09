namespace Trip.Presentation.Contracts.Responses;

public sealed record RiderResponse(
    long Id,
    string FullName,
    string PhoneNumber,
    DateOnly? DateOfBirth,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);

