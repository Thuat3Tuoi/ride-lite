using Trip.Domain.Entities;
using Trip.Domain.Enumerators;
using Trip.Domain.States;
using Xunit;
using TripEntity = Trip.Domain.Entities.Trip;

namespace Trip.Domain.Tests;

public class TripStateTests
{
    private const long ValidRiderId = 1L;
    private const string ValidPickupAddress = "Ben Thanh, District 1, HCMC";
    private const decimal ValidPickupLat = 10.7725m;
    private const decimal ValidPickupLng = 106.6980m;
    private const string ValidDropoffAddress = "Landmark 81, Binh Thanh, HCMC";
    private const decimal ValidDropoffLat = 10.7950m;
    private const decimal ValidDropoffLng = 106.7218m;
    private const decimal ValidCost = 120000m;

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

    [Theory]
    [InlineData(TripStatus.SEARCHING_DRIVER, typeof(SearchingDriverTripState))]
    [InlineData(TripStatus.DRIVER_ACCEPTED, typeof(DriverAcceptedTripState))]
    [InlineData(TripStatus.DRIVER_ARRIVED, typeof(DriverArrivedTripState))]
    [InlineData(TripStatus.IN_PROGRESS, typeof(InProgressTripState))]
    [InlineData(TripStatus.COMPLETED, typeof(CompletedTripState))]
    [InlineData(TripStatus.CANCELLED, typeof(CancelledTripState))]
    [InlineData(TripStatus.NO_DRIVER_FOUND, typeof(NoDriverFoundTripState))]
    public void FromStatus_ShouldReturnCorrectStateInstance(TripStatus status, Type expectedStateType)
    {
        var state = TripState.FromStatus(status);

        Assert.IsType(expectedStateType, state);
        Assert.Equal(status, state.Status);
    }

    [Fact]
    public void NewTrip_InitialState_ShouldBeSearchingDriver()
    {
        var trip = CreateDefaultTrip();

        Assert.IsType<SearchingDriverTripState>(trip.State);
        Assert.Equal(TripStatus.SEARCHING_DRIVER, trip.Status);
    }

    [Fact]
    public void StateTransitions_AlongHappyPath_ShouldUpdateStateAndStatus()
    {
        var trip = CreateDefaultTrip();

        // 1. SearchingDriver -> DriverAccepted
        trip.AcceptDriver(42L);
        Assert.IsType<DriverAcceptedTripState>(trip.State);
        Assert.Equal(TripStatus.DRIVER_ACCEPTED, trip.Status);
        Assert.Equal(42L, trip.DriverId);

        // 2. DriverAccepted -> DriverArrived
        trip.DriverArrive();
        Assert.IsType<DriverArrivedTripState>(trip.State);
        Assert.Equal(TripStatus.DRIVER_ARRIVED, trip.Status);

        // 3. DriverArrived -> InProgress
        trip.StartTrip();
        Assert.IsType<InProgressTripState>(trip.State);
        Assert.Equal(TripStatus.IN_PROGRESS, trip.Status);

        // 4. InProgress -> Completed
        trip.CompleteTrip();
        Assert.IsType<CompletedTripState>(trip.State);
        Assert.Equal(TripStatus.COMPLETED, trip.Status);
    }

    [Fact]
    public void StateTransition_SearchingDriver_To_NoDriverFound()
    {
        var trip = CreateDefaultTrip();

        trip.MarkNoDriverFound();

        Assert.IsType<NoDriverFoundTripState>(trip.State);
        Assert.Equal(TripStatus.NO_DRIVER_FOUND, trip.Status);
    }

    [Theory]
    [InlineData(1)] // Cancel at SearchingDriver
    [InlineData(2)] // Cancel at DriverAccepted
    [InlineData(3)] // Cancel at DriverArrived
    [InlineData(4)] // Cancel at InProgress
    public void Cancel_FromAllowedStates_ShouldTransitionToCancelledState(int stage)
    {
        var trip = CreateDefaultTrip();

        if (stage >= 2) trip.AcceptDriver(10L);
        if (stage >= 3) trip.DriverArrive();
        if (stage >= 4) trip.StartTrip();

        trip.Cancel(CancelledBy.RIDER, "Need to cancel");

        Assert.IsType<CancelledTripState>(trip.State);
        Assert.Equal(TripStatus.CANCELLED, trip.Status);
        Assert.Equal("Need to cancel", trip.CancelReason);
        Assert.Equal(CancelledBy.RIDER, trip.CancelledBy);
    }

    [Fact]
    public void InvalidAction_StartTrip_WhenSearchingDriver_ShouldThrow()
    {
        var trip = CreateDefaultTrip();

        var ex = Assert.Throws<InvalidOperationException>(() => trip.StartTrip());
        Assert.Contains("Cannot start trip when trip status is 'SEARCHING_DRIVER'", ex.Message);
    }

    [Fact]
    public void InvalidAction_CompleteTrip_WhenDriverAccepted_ShouldThrow()
    {
        var trip = CreateDefaultTrip();
        trip.AcceptDriver(10L);

        var ex = Assert.Throws<InvalidOperationException>(() => trip.CompleteTrip());
        Assert.Contains("Cannot complete trip when trip status is 'DRIVER_ACCEPTED'", ex.Message);
    }

    [Fact]
    public void InvalidAction_Cancel_WhenCompleted_ShouldThrow()
    {
        var trip = CreateDefaultTrip();
        trip.AcceptDriver(10L);
        trip.DriverArrive();
        trip.StartTrip();
        trip.CompleteTrip();

        var ex = Assert.Throws<InvalidOperationException>(() => trip.Cancel(CancelledBy.RIDER));
        Assert.Contains("Cannot cancel a trip when status is 'COMPLETED'", ex.Message);
    }

    [Fact]
    public void InvalidAction_Cancel_WhenAlreadyCancelled_ShouldThrow()
    {
        var trip = CreateDefaultTrip();
        trip.Cancel(CancelledBy.SYSTEM);

        var ex = Assert.Throws<InvalidOperationException>(() => trip.Cancel(CancelledBy.RIDER));
        Assert.Contains("Cannot cancel a trip when status is 'CANCELLED'", ex.Message);
    }

    [Fact]
    public void InvalidAction_Cancel_WhenNoDriverFound_ShouldThrow()
    {
        var trip = CreateDefaultTrip();
        trip.MarkNoDriverFound();

        var ex = Assert.Throws<InvalidOperationException>(() => trip.Cancel(CancelledBy.RIDER));
        Assert.Contains("Cannot cancel a trip when status is 'NO_DRIVER_FOUND'", ex.Message);
    }
}

