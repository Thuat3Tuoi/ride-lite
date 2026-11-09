using MatchingPricing.Domain.Enumerators;

namespace MatchingPricing.Application.Features.Driver.Commands.CreateDriver;

public sealed record CreateDriverCommand(
    string FullName,
    string PhoneNumber,
    DateOnly? DateOfBirth = null,
    DriverStatus Status = DriverStatus.offline
);

