using MatchingPricing.Domain.Entities;
using MatchingPricing.Domain.Enumerators;
using Xunit;

namespace MatchingPricing.Domain.Tests;

public class DriverTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateDriver()
    {
        var dob = new DateOnly(1988, 8, 18);
        var driver = Driver.Create("Le Van Driver", "0901234567", dob);

        Assert.Equal("Le Van Driver", driver.FullName);
        Assert.Equal("0901234567", driver.PhoneNumber);
        Assert.Equal(dob, driver.DateOfBirth);
        Assert.Equal(DriverStatus.offline, driver.Status);
        Assert.Null(driver.Latitude);
        Assert.Null(driver.Longitude);
        Assert.Null(driver.Location);
        Assert.Null(driver.LocationUpdatedAt);
        Assert.NotEqual(default, driver.CreatedAt);
        Assert.NotEqual(default, driver.UpdatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidFullName_ShouldThrow(string invalidName)
    {
        Assert.Throws<ArgumentException>(() =>
            Driver.Create(invalidName, "0901234567"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidPhoneNumber_ShouldThrow(string invalidPhone)
    {
        Assert.Throws<ArgumentException>(() =>
            Driver.Create("Driver Name", invalidPhone));
    }

    [Fact]
    public void Create_WithFutureDateOfBirth_ShouldThrow()
    {
        var future = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        Assert.Throws<ArgumentException>(() =>
            Driver.Create("Driver Name", "0901234567", future));
    }

    [Fact]
    public void UpdateLocation_WithValidCoordinates_ShouldUpdateLocationProperties()
    {
        var driver = Driver.Create("Driver", "0901234567");

        driver.UpdateLocation(10.7725m, 106.6980m);

        Assert.Equal(10.7725m, driver.Latitude);
        Assert.Equal(106.6980m, driver.Longitude);
        Assert.NotNull(driver.Location);
        Assert.Contains("POINT(106.6980 10.7725)", driver.Location);
        Assert.NotNull(driver.LocationUpdatedAt);
    }

    [Theory]
    [InlineData(-91, 106)]
    [InlineData(91, 106)]
    [InlineData(10, -181)]
    [InlineData(10, 181)]
    public void UpdateLocation_WithInvalidCoordinates_ShouldThrow(decimal lat, decimal lng)
    {
        var driver = Driver.Create("Driver", "0901234567");

        Assert.Throws<ArgumentException>(() => driver.UpdateLocation(lat, lng));
    }

    [Fact]
    public void StatusTransitions_ShouldUpdateStatus()
    {
        var driver = Driver.Create("Driver", "0901234567");
        Assert.Equal(DriverStatus.offline, driver.Status);

        driver.GoOnline();
        Assert.Equal(DriverStatus.available, driver.Status);

        driver.SetBusy();
        Assert.Equal(DriverStatus.busy, driver.Status);

        driver.GoOffline();
        Assert.Equal(DriverStatus.offline, driver.Status);
    }
}

