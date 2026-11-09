using Trip.Domain.Entities;
using Trip.Domain.Enumerators;
using Xunit;
using TripEntity = Trip.Domain.Entities.Trip;

namespace Trip.Domain.Tests;

public class TripTests
{
    private const long ValidRiderId = 1L;
    private const string ValidPickupAddress = "Ben Thanh, District 1, HCMC";
    private const decimal ValidPickupLat = 10.7725m;
    private const decimal ValidPickupLng = 106.6980m;
    private const string ValidDropoffAddress = "Landmark 81, Binh Thanh, HCMC";
    private const decimal ValidDropoffLat = 10.7950m;
    private const decimal ValidDropoffLng = 106.7218m;
    private const decimal ValidCost = 120000m;

    [Fact]
    public void Create_WithValidData_ShouldCreateTripWithSearchingDriverStatus()
    {
        var trip = TripEntity.Create(
            ValidRiderId,
            ValidPickupAddress,
            ValidPickupLat,
            ValidPickupLng,
            ValidDropoffAddress,
            ValidDropoffLat,
            ValidDropoffLng,
            ValidCost
        );

        Assert.Equal(ValidRiderId, trip.RiderId);
        Assert.Null(trip.DriverId);
        Assert.Equal(ValidPickupAddress, trip.PickupAddress);
        Assert.Equal(ValidPickupLat, trip.PickupLatitude);
        Assert.Equal(ValidPickupLng, trip.PickupLongitude);
        Assert.Equal(ValidDropoffAddress, trip.DropoffAddress);
        Assert.Equal(ValidDropoffLat, trip.DropoffLatitude);
        Assert.Equal(ValidDropoffLng, trip.DropoffLongitude);
        Assert.Equal(ValidCost, trip.Cost);
        Assert.Equal(TripStatus.SEARCHING_DRIVER, trip.Status);
        Assert.NotEqual(default, trip.CreatedAt);
        Assert.NotEqual(default, trip.UpdatedAt);
        Assert.Null(trip.AcceptedAt);
        Assert.Null(trip.DriverArrivedAt);
        Assert.Null(trip.StartedAt);
        Assert.Null(trip.CompletedAt);
        Assert.Null(trip.CancelledAt);
        Assert.Null(trip.CancelledBy);
        Assert.Null(trip.CancelReason);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidRiderId_ShouldThrow(long invalidRiderId)
    {
        Assert.Throws<ArgumentException>(() =>
            TripEntity.Create(
                invalidRiderId,
                ValidPickupAddress,
                ValidPickupLat,
                ValidPickupLng,
                ValidDropoffAddress,
                ValidDropoffLat,
                ValidDropoffLng,
                ValidCost
            ));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyPickupAddress_ShouldThrow(string emptyAddress)
    {
        Assert.Throws<ArgumentException>(() =>
            TripEntity.Create(
                ValidRiderId,
                emptyAddress,
                ValidPickupLat,
                ValidPickupLng,
                ValidDropoffAddress,
                ValidDropoffLat,
                ValidDropoffLng,
                ValidCost
            ));
    }

    [Fact]
    public void Create_WithTooLongPickupAddress_ShouldThrow()
    {
        var longAddress = new string('A', 501);
        Assert.Throws<ArgumentException>(() =>
            TripEntity.Create(
                ValidRiderId,
                longAddress,
                ValidPickupLat,
                ValidPickupLng,
                ValidDropoffAddress,
                ValidDropoffLat,
                ValidDropoffLng,
                ValidCost
            ));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyDropoffAddress_ShouldThrow(string emptyAddress)
    {
        Assert.Throws<ArgumentException>(() =>
            TripEntity.Create(
                ValidRiderId,
                ValidPickupAddress,
                ValidPickupLat,
                ValidPickupLng,
                emptyAddress,
                ValidDropoffLat,
                ValidDropoffLng,
                ValidCost
            ));
    }

    [Fact]
    public void Create_WithTooLongDropoffAddress_ShouldThrow()
    {
        var longAddress = new string('B', 501);
        Assert.Throws<ArgumentException>(() =>
            TripEntity.Create(
                ValidRiderId,
                ValidPickupAddress,
                ValidPickupLat,
                ValidPickupLng,
                longAddress,
                ValidDropoffLat,
                ValidDropoffLng,
                ValidCost
            ));
    }

    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    public void Create_WithInvalidLatitude_ShouldThrow(decimal invalidLat)
    {
        Assert.Throws<ArgumentException>(() =>
            TripEntity.Create(
                ValidRiderId,
                ValidPickupAddress,
                invalidLat,
                ValidPickupLng,
                ValidDropoffAddress,
                ValidDropoffLat,
                ValidDropoffLng,
                ValidCost
            ));
    }

    [Theory]
    [InlineData(-181)]
    [InlineData(181)]
    public void Create_WithInvalidLongitude_ShouldThrow(decimal invalidLng)
    {
        Assert.Throws<ArgumentException>(() =>
            TripEntity.Create(
                ValidRiderId,
                ValidPickupAddress,
                ValidPickupLat,
                invalidLng,
                ValidDropoffAddress,
                ValidDropoffLat,
                ValidDropoffLng,
                ValidCost
            ));
    }

    [Fact]
    public void Create_WithNegativeCost_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() =>
            TripEntity.Create(
                ValidRiderId,
                ValidPickupAddress,
                ValidPickupLat,
                ValidPickupLng,
                ValidDropoffAddress,
                ValidDropoffLat,
                ValidDropoffLng,
                -1m
            ));
    }

    [Fact]
    public void Create_WithZeroCost_ShouldSucceed()
    {
        var trip = TripEntity.Create(
            ValidRiderId,
            ValidPickupAddress,
            ValidPickupLat,
            ValidPickupLng,
            ValidDropoffAddress,
            ValidDropoffLat,
            ValidDropoffLng,
            0m
        );

        Assert.Equal(0m, trip.Cost);
    }

    [Fact]
    public void AcceptDriver_WhenSearchingDriver_ShouldTransitionToDriverAccepted()
    {
        var trip = CreateDefaultTrip();
        var driverId = 99L;

        trip.AcceptDriver(driverId);

        Assert.Equal(TripStatus.DRIVER_ACCEPTED, trip.Status);
        Assert.Equal(driverId, trip.DriverId);
        Assert.NotNull(trip.AcceptedAt);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AcceptDriver_WithInvalidDriverId_ShouldThrow(long invalidDriverId)
    {
        var trip = CreateDefaultTrip();

        Assert.Throws<ArgumentException>(() => trip.AcceptDriver(invalidDriverId));
    }

    [Fact]
    public void DriverArrive_WhenDriverAccepted_ShouldTransitionToDriverArrived()
    {
        var trip = CreateDefaultTrip();
        trip.AcceptDriver(99L);

        trip.DriverArrive();

        Assert.Equal(TripStatus.DRIVER_ARRIVED, trip.Status);
        Assert.NotNull(trip.DriverArrivedAt);
    }

    [Fact]
    public void StartTrip_WhenDriverArrived_ShouldTransitionToInProgress()
    {
        var trip = CreateDefaultTrip();
        trip.AcceptDriver(99L);
        trip.DriverArrive();

        trip.StartTrip();

        Assert.Equal(TripStatus.IN_PROGRESS, trip.Status);
        Assert.NotNull(trip.StartedAt);
    }

    [Fact]
    public void CompleteTrip_WhenInProgress_ShouldTransitionToCompleted()
    {
        var trip = CreateDefaultTrip();
        trip.AcceptDriver(99L);
        trip.DriverArrive();
        trip.StartTrip();

        trip.CompleteTrip();

        Assert.Equal(TripStatus.COMPLETED, trip.Status);
        Assert.NotNull(trip.CompletedAt);
    }

    [Fact]
    public void Cancel_WhenSearchingDriver_ShouldTransitionToCancelled()
    {
        var trip = CreateDefaultTrip();

        trip.Cancel(CancelledBy.RIDER, "Changed my mind");

        Assert.Equal(TripStatus.CANCELLED, trip.Status);
        Assert.Equal(CancelledBy.RIDER, trip.CancelledBy);
        Assert.Equal("Changed my mind", trip.CancelReason);
        Assert.NotNull(trip.CancelledAt);
    }

    [Fact]
    public void Cancel_WhenDriverAccepted_ShouldTransitionToCancelled()
    {
        var trip = CreateDefaultTrip();
        trip.AcceptDriver(99L);

        trip.Cancel(CancelledBy.DRIVER, "Flat tire");

        Assert.Equal(TripStatus.CANCELLED, trip.Status);
        Assert.Equal(CancelledBy.DRIVER, trip.CancelledBy);
        Assert.Equal("Flat tire", trip.CancelReason);
        Assert.NotNull(trip.CancelledAt);
    }

    [Fact]
    public void Cancel_WhenCompleted_ShouldThrow()
    {
        var trip = CreateDefaultTrip();
        trip.AcceptDriver(99L);
        trip.DriverArrive();
        trip.StartTrip();
        trip.CompleteTrip();

        Assert.Throws<InvalidOperationException>(() =>
            trip.Cancel(CancelledBy.RIDER, "Too late"));
    }

    [Fact]
    public void Cancel_WhenAlreadyCancelled_ShouldThrow()
    {
        var trip = CreateDefaultTrip();
        trip.Cancel(CancelledBy.RIDER);

        Assert.Throws<InvalidOperationException>(() =>
            trip.Cancel(CancelledBy.SYSTEM));
    }

    [Fact]
    public void MarkNoDriverFound_WhenSearchingDriver_ShouldTransitionToNoDriverFound()
    {
        var trip = CreateDefaultTrip();

        trip.MarkNoDriverFound();

        Assert.Equal(TripStatus.NO_DRIVER_FOUND, trip.Status);
    }

    [Fact]
    public void MarkNoDriverFound_WhenDriverAccepted_ShouldThrow()
    {
        var trip = CreateDefaultTrip();
        trip.AcceptDriver(99L);

        Assert.Throws<InvalidOperationException>(() => trip.MarkNoDriverFound());
    }

    [Fact]
    public void FullLifecycle_HappyPath_ShouldSucceed()
    {
        var trip = CreateDefaultTrip();
        Assert.Equal(TripStatus.SEARCHING_DRIVER, trip.Status);

        trip.AcceptDriver(42L);
        Assert.Equal(TripStatus.DRIVER_ACCEPTED, trip.Status);

        trip.DriverArrive();
        Assert.Equal(TripStatus.DRIVER_ARRIVED, trip.Status);

        trip.StartTrip();
        Assert.Equal(TripStatus.IN_PROGRESS, trip.Status);

        trip.CompleteTrip();
        Assert.Equal(TripStatus.COMPLETED, trip.Status);
    }

    private static TripEntity CreateDefaultTrip()
    {
        return TripEntity.Create(
            ValidRiderId,
            ValidPickupAddress,
            ValidPickupLat,
            ValidPickupLng,
            ValidDropoffAddress,
            ValidDropoffLat,
            ValidDropoffLng,
            ValidCost
        );
    }
}

