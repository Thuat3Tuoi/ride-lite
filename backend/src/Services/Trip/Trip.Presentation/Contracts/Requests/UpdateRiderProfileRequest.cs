namespace Trip.Presentation.Contracts.Requests;

public sealed record UpdateRiderProfileRequest(
    string FullName,
    string PhoneNumber,
    DateOnly? DateOfBirth = null
);

