using Trip.Application.Features.Rider.Commands.CreateRider;
using Trip.Application.Features.Rider.Commands.UpdateRiderProfile;
using Trip.Domain.Errors;
using Xunit;

namespace Trip.Application.Tests.Features.Rider;

public class RiderCommandValidatorTests
{
    private readonly CreateRiderCommandValidator _createValidator = new();
    private readonly UpdateRiderProfileCommandValidator _updateValidator = new();

    [Fact]
    public void CreateRider_WithValidCommand_ShouldBeValid()
    {
        var command = new CreateRiderCommand("Nguyen Van A", "0912345678", new DateOnly(1995, 1, 1));
        var result = _createValidator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateRider_WithEmptyName_ShouldHaveInvalidFullNameError(string name)
    {
        var command = new CreateRiderCommand(name, "0912345678");
        var result = _createValidator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Equal(TripErrors.InvalidFullName, result.Errors[0].CustomState);
    }

    [Fact]
    public void CreateRider_WithNameExceeding255Chars_ShouldHaveInvalidFullNameError()
    {
        var command = new CreateRiderCommand(new string('a', 256), "0912345678");
        var result = _createValidator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Equal(TripErrors.InvalidFullName, result.Errors[0].CustomState);
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("091234567")]
    [InlineData("1234567890")]
    [InlineData("09123456789")]
    [InlineData("abcdefghij")]
    public void CreateRider_WithInvalidPhone_ShouldHaveInvalidPhoneNumberError(string phone)
    {
        var command = new CreateRiderCommand("Valid Name", phone);
        var result = _createValidator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Equal(TripErrors.InvalidPhoneNumber, result.Errors[0].CustomState);
    }

    [Fact]
    public void CreateRider_WithFutureDateOfBirth_ShouldHaveInvalidDateOfBirthError()
    {
        var futureDob = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var command = new CreateRiderCommand("Valid Name", "0912345678", futureDob);
        var result = _createValidator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Equal(TripErrors.InvalidDateOfBirth, result.Errors[0].CustomState);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void UpdateRiderProfile_WithInvalidId_ShouldHaveRiderNotFoundError(long invalidId)
    {
        var command = new UpdateRiderProfileCommand(invalidId, "Valid Name", "0912345678");
        var result = _updateValidator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Equal(TripErrors.RiderNotFound, result.Errors[0].CustomState);
    }
}

