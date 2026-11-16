using Trip.Domain.Enumerators;
using Trip.Domain.States;
using Xunit;

namespace Trip.Domain.Tests;

public class TripStateFactoryTests
{
    private readonly ITripStateFactory _factory = TripStateFactory.Instance;

    [Theory]
    [InlineData(TripStatus.SEARCHING_DRIVER, typeof(SearchingDriverTripState))]
    [InlineData(TripStatus.DRIVER_ACCEPTED, typeof(DriverAcceptedTripState))]
    [InlineData(TripStatus.DRIVER_ARRIVED, typeof(DriverArrivedTripState))]
    [InlineData(TripStatus.IN_PROGRESS, typeof(InProgressTripState))]
    [InlineData(TripStatus.COMPLETED, typeof(CompletedTripState))]
    [InlineData(TripStatus.CANCELLED, typeof(CancelledTripState))]
    [InlineData(TripStatus.NO_DRIVER_FOUND, typeof(NoDriverFoundTripState))]
    public void GetState_ShouldReturnCorrectStateTypeAndMatchingStatus(TripStatus status, Type expectedStateType)
    {
        var state = _factory.GetState(status);

        Assert.NotNull(state);
        Assert.IsType(expectedStateType, state);
        Assert.Equal(status, state.Status);
    }

    [Theory]
    [InlineData(TripStatus.SEARCHING_DRIVER)]
    [InlineData(TripStatus.DRIVER_ACCEPTED)]
    [InlineData(TripStatus.DRIVER_ARRIVED)]
    [InlineData(TripStatus.IN_PROGRESS)]
    [InlineData(TripStatus.COMPLETED)]
    [InlineData(TripStatus.CANCELLED)]
    [InlineData(TripStatus.NO_DRIVER_FOUND)]
    public void GetState_ShouldReturnFlyweightCachedInstance_WhenCalledMultipleTimes(TripStatus status)
    {
        var firstCall = _factory.GetState(status);
        var secondCall = _factory.GetState(status);

        Assert.Same(firstCall, secondCall);
    }

    [Fact]
    public void Properties_ShouldReturnMatchingStateInstances()
    {
        Assert.Same(_factory.GetState(TripStatus.SEARCHING_DRIVER), _factory.SearchingDriver);
        Assert.Same(_factory.GetState(TripStatus.DRIVER_ACCEPTED), _factory.DriverAccepted);
        Assert.Same(_factory.GetState(TripStatus.DRIVER_ARRIVED), _factory.DriverArrived);
        Assert.Same(_factory.GetState(TripStatus.IN_PROGRESS), _factory.InProgress);
        Assert.Same(_factory.GetState(TripStatus.COMPLETED), _factory.Completed);
        Assert.Same(_factory.GetState(TripStatus.CANCELLED), _factory.Cancelled);
        Assert.Same(_factory.GetState(TripStatus.NO_DRIVER_FOUND), _factory.NoDriverFound);
    }

    [Fact]
    public void GetState_ShouldThrowArgumentOutOfRangeException_WhenStatusIsInvalid()
    {
        var invalidStatus = (TripStatus)999;

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => _factory.GetState(invalidStatus));
        Assert.Equal("status", ex.ParamName);
    }

    [Fact]
    public void TripState_FromStatus_ShouldDelegateToTripStateFactory()
    {
        var stateFromClass = TripState.FromStatus(TripStatus.IN_PROGRESS);
        var stateFromFactory = _factory.GetState(TripStatus.IN_PROGRESS);

        Assert.Same(stateFromFactory, stateFromClass);
    }
}

