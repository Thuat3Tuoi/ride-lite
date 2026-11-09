namespace MatchingPricing.Application.Features.Driver.Commands.CreateDriver;

public sealed record CreateDriverResult(
    long Id,
    string FullName,
    string PhoneNumber,
    DateOnly? DateOfBirth,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);

