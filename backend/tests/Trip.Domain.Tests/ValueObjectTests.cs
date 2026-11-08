using Trip.Domain.ValueObjects;

namespace Trip.Domain.Tests;

public class ValueObjectTests
{
    [Theory]
    [InlineData(-90.1)]
    [InlineData(90.1)]
    public void GeoPoint_RejectsInvalidLatitude(double latitude)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new GeoPoint((decimal)latitude, 0m));
    }

    [Theory]
    [InlineData(-180.1)]
    [InlineData(180.1)]
    public void GeoPoint_RejectsInvalidLongitude(double longitude)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new GeoPoint(0m, (decimal)longitude));
    }

    [Fact]
    public void GeoPoint_AcceptsValidCoordinates()
    {
        var point = new GeoPoint(10.7769m, 106.7009m);

        Assert.Equal(10.7769m, point.Latitude);
        Assert.Equal(106.7009m, point.Longitude);
    }

    [Fact]
    public void Money_RejectsNegativeAmount()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Money(-1m));
    }

    [Fact]
    public void Money_RejectsFractionalAmount()
    {
        Assert.Throws<ArgumentException>(() => new Money(1000.5m));
    }

    [Fact]
    public void Money_ExposesVndCurrency()
    {
        var money = new Money(150000m);

        Assert.Equal(150000m, money.Amount);
        Assert.Equal("VND", money.Currency);
    }
}
