namespace Trip.Domain.Entities;

public sealed class Rider
{
    public long Id { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public DateOnly? DateOfBirth { get; private set; }
    public string PhoneNumber { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public IReadOnlyCollection<Trip> Trips => _trips.AsReadOnly();
    private readonly List<Trip> _trips = [];

    private Rider() { }

    public static Rider Create(
        string fullName,
        string phoneNumber,
        DateOnly? dateOfBirth = null)
    {
        ValidateFullName(fullName);
        ValidatePhoneNumber(phoneNumber);
        ValidateDateOfBirth(dateOfBirth);

        var now = DateTimeOffset.UtcNow;

        return new Rider
        {
            FullName = fullName.Trim(),
            PhoneNumber = phoneNumber.Trim(),
            DateOfBirth = dateOfBirth,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void UpdateProfile(
        string fullName,
        string phoneNumber,
        DateOnly? dateOfBirth = null)
    {
        ValidateFullName(fullName);
        ValidatePhoneNumber(phoneNumber);
        ValidateDateOfBirth(dateOfBirth);

        FullName = fullName.Trim();
        PhoneNumber = phoneNumber.Trim();
        DateOfBirth = dateOfBirth;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private static void ValidateFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be empty.");

        if (fullName.Trim().Length > 255)
            throw new ArgumentException("Full name cannot exceed 255 characters.");
    }

    private static void ValidatePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty.");

        var trimmed = phoneNumber.Trim();
        if (trimmed.Length > 20)
            throw new ArgumentException("Phone number cannot exceed 20 characters.");
    }

    private static void ValidateDateOfBirth(DateOnly? dateOfBirth)
    {
        if (dateOfBirth.HasValue)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (dateOfBirth.Value > today)
                throw new ArgumentException("Date of birth cannot be in the future.");
        }
    }
}

