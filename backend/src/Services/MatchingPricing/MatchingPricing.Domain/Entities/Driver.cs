using System.Globalization;
using MatchingPricing.Domain.Enumerators;

namespace MatchingPricing.Domain.Entities;

public sealed class Driver
{
    public long Id { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public DateOnly? DateOfBirth { get; private set; }
    public string PhoneNumber { get; private set; } = string.Empty;
    public decimal? Latitude { get; private set; }
    public decimal? Longitude { get; private set; }
    public string? Location { get; private set; }
    public DriverStatus Status { get; private set; }
    public DateTimeOffset? LocationUpdatedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public IReadOnlyCollection<DriverAssignment> Assignments => _assignments.AsReadOnly();
    private readonly List<DriverAssignment> _assignments = [];

    private Driver() { }

    public static Driver Create(
        string fullName,
        string phoneNumber,
        DateOnly? dateOfBirth = null,
        DriverStatus status = DriverStatus.offline)
    {
        ValidateFullName(fullName);
        ValidatePhoneNumber(phoneNumber);
        ValidateDateOfBirth(dateOfBirth);

        var now = DateTimeOffset.UtcNow;

        return new Driver
        {
            FullName = fullName.Trim(),
            PhoneNumber = phoneNumber.Trim(),
            DateOfBirth = dateOfBirth,
            Status = status,
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

    public void UpdateLocation(decimal latitude, decimal longitude)
    {
        if (latitude is < -90 or > 90)
            throw new ArgumentException("Latitude must be between -90 and 90.");

        if (longitude is < -180 or > 180)
            throw new ArgumentException("Longitude must be between -180 and 180.");

        var now = DateTimeOffset.UtcNow;
        Latitude = latitude;
        Longitude = longitude;
        // WGS84 point format: POINT(lon lat)
        Location = string.Create(CultureInfo.InvariantCulture, $"SRID=4326;POINT({longitude} {latitude})");
        LocationUpdatedAt = now;
        UpdatedAt = now;
    }

    public void SetStatus(DriverStatus status)
    {
        Status = status;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void GoOnline() => SetStatus(DriverStatus.available);
    public void GoOffline() => SetStatus(DriverStatus.offline);
    public void SetBusy() => SetStatus(DriverStatus.busy);

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

