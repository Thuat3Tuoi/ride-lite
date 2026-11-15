using MatchingPricing.Application.Features.Driver.Commands.CreateDriver;
using MatchingPricing.Application.Features.Driver.Commands.UpdateDriverLocation;
using MatchingPricing.Application.Features.Driver.Commands.UpdateDriverStatus;
using MatchingPricing.Application.Features.Driver.Queries.FindNearbyDrivers;
using MatchingPricing.Application.Features.Driver.Queries.GetDriverById;
using MatchingPricing.Domain.Enumerators;
using MatchingPricing.Domain.Errors;
using Xunit;

namespace MatchingPricing.Application.Tests.Features.Driver;

public sealed class DriverCommandValidatorTests
{
    private readonly CreateDriverCommandValidator _createValidator = new();
    private readonly UpdateDriverLocationCommandValidator _updateLocationValidator = new();
    private readonly UpdateDriverStatusCommandValidator _updateStatusValidator = new();
    private readonly GetDriverByIdQueryValidator _getByIdValidator = new();
    private readonly FindNearbyDriversQueryValidator _findNearbyValidator = new();

    [Fact]
    public void CreateDriver_WithValidData_ShouldBeValid()
    {
        var command = new CreateDriverCommand("Nguyen Van A", "0901234567", new DateOnly(1995, 5, 20));
        var result = _createValidator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void CreateDriver_WithInvalidFullName_ShouldHaveInvalidFullNameError(string? fullName)
    {
        var command = new CreateDriverCommand(fullName!, "0901234567");
        var result = _createValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(MatchingPricingErrors.InvalidFullName, result.Errors[0].CustomState);
    }

    [Fact]
    public void CreateDriver_WithTooLongFullName_ShouldHaveInvalidFullNameError()
    {
        var command = new CreateDriverCommand(new string('a', 256), "0901234567");
        var result = _createValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(MatchingPricingErrors.InvalidFullName, result.Errors[0].CustomState);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("123456789")]
    [InlineData("090123456")]
    [InlineData("09012345678")]
    [InlineData("abcdefghij")]
    public void CreateDriver_WithInvalidPhone_ShouldHaveInvalidPhoneNumberError(string? phone)
    {
        var command = new CreateDriverCommand("Nguyen Van A", phone!);
        var result = _createValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(MatchingPricingErrors.InvalidPhoneNumber, result.Errors[0].CustomState);
    }

    [Fact]
    public void CreateDriver_WithFutureDateOfBirth_ShouldHaveInvalidDateOfBirthError()
    {
        var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var command = new CreateDriverCommand("Nguyen Van A", "0901234567", futureDate);
        var result = _createValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(MatchingPricingErrors.InvalidDateOfBirth, result.Errors[0].CustomState);
    }

    [Fact]
    public void CreateDriver_WithInvalidStatus_ShouldHaveInvalidParametersError()
    {
        var command = new CreateDriverCommand("Nguyen Van A", "0901234567", null, (DriverStatus)999);
        var result = _createValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(MatchingPricingErrors.InvalidParameters, result.Errors[0].CustomState);
    }

    [Fact]
    public void UpdateDriverLocation_WithValidData_ShouldBeValid()
    {
        var command = new UpdateDriverLocationCommand(1, 10.7725m, 106.6980m);
        var result = _updateLocationValidator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void UpdateDriverLocation_WithInvalidDriverId_ShouldHaveInvalidDriverIdError(long driverId)
    {
        var command = new UpdateDriverLocationCommand(driverId, 10.7725m, 106.6980m);
        var result = _updateLocationValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(MatchingPricingErrors.InvalidDriverId, result.Errors[0].CustomState);
    }

    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    public void UpdateDriverLocation_WithInvalidLatitude_ShouldHaveInvalidCoordinatesError(decimal lat)
    {
        var command = new UpdateDriverLocationCommand(1, lat, 106.6980m);
        var result = _updateLocationValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(MatchingPricingErrors.InvalidCoordinates, result.Errors[0].CustomState);
    }

    [Theory]
    [InlineData(-181)]
    [InlineData(181)]
    public void UpdateDriverLocation_WithInvalidLongitude_ShouldHaveInvalidCoordinatesError(decimal lon)
    {
        var command = new UpdateDriverLocationCommand(1, 10.7725m, lon);
        var result = _updateLocationValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(MatchingPricingErrors.InvalidCoordinates, result.Errors[0].CustomState);
    }

    [Fact]
    public void UpdateDriverStatus_WithValidData_ShouldBeValid()
    {
        var command = new UpdateDriverStatusCommand(1, DriverStatus.available);
        var result = _updateStatusValidator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void UpdateDriverStatus_WithInvalidDriverId_ShouldHaveInvalidDriverIdError(long driverId)
    {
        var command = new UpdateDriverStatusCommand(driverId, DriverStatus.available);
        var result = _updateStatusValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(MatchingPricingErrors.InvalidDriverId, result.Errors[0].CustomState);
    }

    [Fact]
    public void UpdateDriverStatus_WithInvalidStatus_ShouldHaveInvalidParametersError()
    {
        var command = new UpdateDriverStatusCommand(1, (DriverStatus)999);
        var result = _updateStatusValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(MatchingPricingErrors.InvalidParameters, result.Errors[0].CustomState);
    }

    [Fact]
    public void GetDriverById_WithValidId_ShouldBeValid()
    {
        var query = new GetDriverByIdQuery(1);
        var result = _getByIdValidator.Validate(query);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GetDriverById_WithInvalidId_ShouldHaveDriverNotFoundError(long id)
    {
        var query = new GetDriverByIdQuery(id);
        var result = _getByIdValidator.Validate(query);
        Assert.False(result.IsValid);
        Assert.Equal(MatchingPricingErrors.DriverNotFound, result.Errors[0].CustomState);
    }

    [Fact]
    public void FindNearbyDrivers_WithValidData_ShouldBeValid()
    {
        var query = new FindNearbyDriversQuery(10.7725m, 106.6980m, 5.0, 10);
        var result = _findNearbyValidator.Validate(query);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void FindNearbyDrivers_WithInvalidRadius_ShouldHaveInvalidRadiusError(double radius)
    {
        var query = new FindNearbyDriversQuery(10.7725m, 106.6980m, radius, 10);
        var result = _findNearbyValidator.Validate(query);
        Assert.False(result.IsValid);
        Assert.Equal(MatchingPricingErrors.InvalidRadius, result.Errors[0].CustomState);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void FindNearbyDrivers_WithInvalidLimit_ShouldHaveInvalidParametersError(int limit)
    {
        var query = new FindNearbyDriversQuery(10.7725m, 106.6980m, 5.0, limit);
        var result = _findNearbyValidator.Validate(query);
        Assert.False(result.IsValid);
        Assert.Equal(MatchingPricingErrors.InvalidParameters, result.Errors[0].CustomState);
    }
}

