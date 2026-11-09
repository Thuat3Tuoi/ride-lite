using Trip.Domain.Entities;
using Xunit;

namespace Trip.Domain.Tests;

public class RiderTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateRider()
    {
        var fullName = "Nguyen Van A";
        var phoneNumber = "0912345678";
        var dob = new DateOnly(1995, 5, 20);

        var rider = Rider.Create(fullName, phoneNumber, dob);

        Assert.Equal("Nguyen Van A", rider.FullName);
        Assert.Equal("0912345678", rider.PhoneNumber);
        Assert.Equal(dob, rider.DateOfBirth);
        Assert.NotEqual(default, rider.CreatedAt);
        Assert.NotEqual(default, rider.UpdatedAt);
        Assert.Empty(rider.Trips);
    }

    [Fact]
    public void Create_WithoutDateOfBirth_ShouldSucceed()
    {
        var rider = Rider.Create("Tran Thi B", "0987654321");

        Assert.Equal("Tran Thi B", rider.FullName);
        Assert.Equal("0987654321", rider.PhoneNumber);
        Assert.Null(rider.DateOfBirth);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyFullName_ShouldThrow(string emptyName)
    {
        Assert.Throws<ArgumentException>(() =>
            Rider.Create(emptyName, "0912345678"));
    }

    [Fact]
    public void Create_WithTooLongFullName_ShouldThrow()
    {
        var longName = new string('a', 256);
        Assert.Throws<ArgumentException>(() =>
            Rider.Create(longName, "0912345678"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyPhoneNumber_ShouldThrow(string emptyPhone)
    {
        Assert.Throws<ArgumentException>(() =>
            Rider.Create("Nguyen Van A", emptyPhone));
    }

    [Fact]
    public void Create_WithTooLongPhoneNumber_ShouldThrow()
    {
        var longPhone = new string('1', 21);
        Assert.Throws<ArgumentException>(() =>
            Rider.Create("Nguyen Van A", longPhone));
    }

    [Fact]
    public void Create_WithFutureDateOfBirth_ShouldThrow()
    {
        var futureDob = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        Assert.Throws<ArgumentException>(() =>
            Rider.Create("Nguyen Van A", "0912345678", futureDob));
    }

    [Fact]
    public void UpdateProfile_WithValidData_ShouldUpdateProperties()
    {
        var rider = Rider.Create("Nguyen Van A", "0912345678");
        var updatedDob = new DateOnly(1990, 1, 1);

        rider.UpdateProfile("Nguyen Van B", "0909999999", updatedDob);

        Assert.Equal("Nguyen Van B", rider.FullName);
        Assert.Equal("0909999999", rider.PhoneNumber);
        Assert.Equal(updatedDob, rider.DateOfBirth);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateProfile_WithInvalidFullName_ShouldThrow(string emptyName)
    {
        var rider = Rider.Create("Nguyen Van A", "0912345678");

        Assert.Throws<ArgumentException>(() =>
            rider.UpdateProfile(emptyName, "0912345678"));
    }

    [Fact]
    public void UpdateProfile_WithFutureDateOfBirth_ShouldThrow()
    {
        var rider = Rider.Create("Nguyen Van A", "0912345678");
        var futureDob = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));

        Assert.Throws<ArgumentException>(() =>
            rider.UpdateProfile("Nguyen Van A", "0912345678", futureDob));
    }
}

