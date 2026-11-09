namespace Trip.Presentation.Contracts.Requests;

public sealed record CreateRiderRequest(
    string FullName,
    string PhoneNumber,
    DateOnly? DateOfBirth = null
);

