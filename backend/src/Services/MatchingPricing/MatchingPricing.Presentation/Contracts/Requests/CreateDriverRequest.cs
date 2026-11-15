using MatchingPricing.Domain.Enumerators;

namespace MatchingPricing.Presentation.Contracts.Requests;

public sealed record CreateDriverRequest(
    string FullName,
    string PhoneNumber,
    DateOnly? DateOfBirth = null,
    DriverStatus Status = DriverStatus.offline
);

