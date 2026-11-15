using Trip.Application.Features.Trip.Commands.AcceptTripDriver;
using Trip.Application.Features.Trip.Commands.CancelTrip;
using Trip.Application.Features.Trip.Commands.CreateTrip;
using Trip.Application.Features.Trip.Commands.StartTrip;
using Trip.Domain.Errors;
using Xunit;

namespace Trip.Application.Tests.Features.Trip;

public class TripCommandValidatorTests
{
    private readonly CreateTripCommandValidator _createTripValidator = new();
    private readonly AcceptTripDriverCommandValidator _acceptDriverValidator = new();
    private readonly CancelTripCommandValidator _cancelTripValidator = new();
    private readonly StartTripCommandValidator _startTripValidator = new();

    [Fact]
    public void CreateTrip_WithValidCommand_ShouldBeValid()
    {
        var command = new CreateTripCommand(
            RiderId: 1L,
            PickupAddress: "Ben Thanh, D1",
            PickupLatitude: 10.7725m,
            PickupLongitude: 106.6980m,
            DropoffAddress: "Landmark 81, BT",
            DropoffLatitude: 10.7950m,
            DropoffLongitude: 106.7218m,
            ClientFare: 50000m
        );

        var result = _createTripValidator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void CreateTrip_WithInvalidRiderId_ShouldHaveRiderNotFoundError(long riderId)
    {
        var command = new CreateTripCommand(
            RiderId: riderId,
            PickupAddress: "A",
            PickupLatitude: 10m,
            PickupLongitude: 106m,
            DropoffAddress: "B",
            DropoffLatitude: 11m,
            DropoffLongitude: 107m
        );

        var result = _createTripValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(TripErrors.RiderNotFound, result.Errors[0].CustomState);
    }

    [Fact]
    public void CreateTrip_WithNegativeClientFare_ShouldHaveInvalidCostError()
    {
        var command = new CreateTripCommand(
            RiderId: 1L,
            PickupAddress: "A",
            PickupLatitude: 10m,
            PickupLongitude: 106m,
            DropoffAddress: "B",
            DropoffLatitude: 11m,
            DropoffLongitude: 107m,
            ClientFare: -100m
        );

        var result = _createTripValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(TripErrors.InvalidCost, result.Errors[0].CustomState);
    }

    [Theory]
    [InlineData(-91, 106)]
    [InlineData(91, 106)]
    [InlineData(10, -181)]
    [InlineData(10, 181)]
    public void CreateTrip_WithInvalidCoordinates_ShouldHaveInvalidCoordinatesError(decimal lat, decimal lng)
    {
        var command = new CreateTripCommand(
            RiderId: 1L,
            PickupAddress: "A",
            PickupLatitude: lat,
            PickupLongitude: lng,
            DropoffAddress: "B",
            DropoffLatitude: 10.5m,
            DropoffLongitude: 106.5m
        );

        var result = _createTripValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(TripErrors.InvalidCoordinates, result.Errors[0].CustomState);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AcceptTripDriver_WithInvalidDriverId_ShouldHaveInvalidDriverIdError(long driverId)
    {
        var command = new AcceptTripDriverCommand(1L, driverId);
        var result = _acceptDriverValidator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Equal(TripErrors.InvalidDriverId, result.Errors[0].CustomState);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AcceptTripDriver_WithInvalidTripId_ShouldHaveNotFoundError(long tripId)
    {
        var command = new AcceptTripDriverCommand(tripId, 10L);
        var result = _acceptDriverValidator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Equal(TripErrors.NotFound, result.Errors[0].CustomState);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CancelTrip_WithInvalidTripId_ShouldHaveNotFoundError(long tripId)
    {
        var command = new CancelTripCommand(tripId, Domain.Enumerators.CancelledBy.RIDER, "Reason");
        var result = _cancelTripValidator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Equal(TripErrors.NotFound, result.Errors[0].CustomState);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void StartTrip_WithInvalidTripId_ShouldHaveNotFoundError(long tripId)
    {
        var command = new StartTripCommand(tripId);
        var result = _startTripValidator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Equal(TripErrors.NotFound, result.Errors[0].CustomState);
    }
}
