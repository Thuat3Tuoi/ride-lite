using BuildingBlocks.SharedKernel.Exceptions;
using Trip.Application.Features.Trip.Commands.AcceptTripDriver;
using Trip.Application.Features.Trip.Commands.CancelTrip;
using Trip.Application.Features.Trip.Commands.CompleteTrip;
using Trip.Application.Features.Trip.Commands.CreateTrip;
using Trip.Application.Features.Trip.Commands.DriverArrive;
using Trip.Application.Features.Trip.Commands.MarkNoDriverFound;
using Trip.Application.Features.Trip.Commands.StartTrip;
using Trip.Application.Features.Trip.Queries.GetTripById;
using Trip.Application.Features.Trip.Queries.GetTripsByDriverId;
using Trip.Application.Features.Trip.Queries.GetTripsByRiderId;
using Trip.Application.Features.Trip.Queries.GetTripsByStatus;
using Trip.Application.Tests.Fakes;
using Trip.Domain.Entities;
using Trip.Domain.Enumerators;
using Trip.Domain.Errors;
using Xunit;
using RiderEntity = Trip.Domain.Entities.Rider;
using TripEntity = Trip.Domain.Entities.Trip;

namespace Trip.Application.Tests.Features.Trip;

public class TripHandlerTests
{
    private readonly FakeTripRepository _tripRepo = new();
    private readonly FakeRiderRepository _riderRepo = new();
    private readonly FakePricingService _pricingService = new();

    private async Task<RiderEntity> SeedRiderAsync()
    {
        var rider = RiderEntity.Create("Test Rider", "0912345678");
        await _riderRepo.AddAsync(rider);
        return rider;
    }

    [Fact]
    public async Task CreateTrip_WithValidData_ShouldCalculateAuthoritativeFareAndCreateTrip()
    {
        var rider = await SeedRiderAsync();
        _pricingService.FixedFare = 85000m;

        var handler = new CreateTripHandler(_tripRepo, _riderRepo, _pricingService);
        var command = new CreateTripCommand(
            RiderId: rider.Id,
            PickupAddress: "Ben Thanh, District 1, HCMC",
            PickupLatitude: 10.7725m,
            PickupLongitude: 106.6980m,
            DropoffAddress: "Landmark 81, Binh Thanh, HCMC",
            DropoffLatitude: 10.7950m,
            DropoffLongitude: 106.7218m,
            ClientFare: 50000m // Client offers lower fare
        );

        var result = await handler.HandleAsync(command);

        Assert.True(result.Id > 0);
        Assert.True(_pricingService.WasCalled);
        // Ensure authoritative fare was persisted, not client amount!
        Assert.Equal(85000m, result.Cost);
        Assert.Equal(TripStatus.SEARCHING_DRIVER.ToString(), result.Status);
        Assert.True(_tripRepo.Saved);
    }

    [Fact]
    public async Task CreateTrip_WhenRiderNotFound_ShouldThrowNotFound()
    {
        var handler = new CreateTripHandler(_tripRepo, _riderRepo, _pricingService);
        var command = new CreateTripCommand(
            RiderId: 9999,
            PickupAddress: "Ben Thanh, District 1, HCMC",
            PickupLatitude: 10.7725m,
            PickupLongitude: 106.6980m,
            DropoffAddress: "Landmark 81, Binh Thanh, HCMC",
            DropoffLatitude: 10.7950m,
            DropoffLongitude: 106.7218m
        );

        var ex = await Assert.ThrowsAsync<AppException>(() => handler.HandleAsync(command));
        Assert.Equal(TripErrors.RiderNotFound.Code, ex.Error.Code);
    }

    [Fact]
    public async Task AcceptTripDriver_WhenValid_ShouldTransitionToDriverAccepted()
    {
        var rider = await SeedRiderAsync();
        var trip = TripEntity.Create(rider.Id, "A", 10.77m, 106.69m, "B", 10.79m, 106.72m, 50000m);
        await _tripRepo.AddAsync(trip);

        var handler = new AcceptTripDriverHandler(_tripRepo);
        var result = await handler.HandleAsync(new AcceptTripDriverCommand(trip.Id, 101L));

        Assert.Equal(101L, result.DriverId);
        Assert.Equal(TripStatus.DRIVER_ACCEPTED.ToString(), result.Status);
        Assert.True(_tripRepo.Saved);
    }

    [Fact]
    public async Task AcceptTripDriver_WhenTripNotFound_ShouldThrowNotFound()
    {
        var handler = new AcceptTripDriverHandler(_tripRepo);
        var ex = await Assert.ThrowsAsync<AppException>(() =>
            handler.HandleAsync(new AcceptTripDriverCommand(999, 101L)));

        Assert.Equal(TripErrors.NotFound.Code, ex.Error.Code);
    }

    [Fact]
    public async Task DriverArrive_WhenValid_ShouldTransitionToDriverArrived()
    {
        var rider = await SeedRiderAsync();
        var trip = TripEntity.Create(rider.Id, "A", 10.77m, 106.69m, "B", 10.79m, 106.72m, 50000m);
        trip.AcceptDriver(101L);
        await _tripRepo.AddAsync(trip);

        var handler = new DriverArriveHandler(_tripRepo);
        var result = await handler.HandleAsync(new DriverArriveCommand(trip.Id));

        Assert.Equal(TripStatus.DRIVER_ARRIVED.ToString(), result.Status);
    }

    [Fact]
    public async Task StartTrip_WhenValid_ShouldTransitionToInProgress()
    {
        var rider = await SeedRiderAsync();
        var trip = TripEntity.Create(rider.Id, "A", 10.77m, 106.69m, "B", 10.79m, 106.72m, 50000m);
        trip.AcceptDriver(101L);
        trip.DriverArrive();
        await _tripRepo.AddAsync(trip);

        var handler = new StartTripHandler(_tripRepo);
        var result = await handler.HandleAsync(new StartTripCommand(trip.Id));

        Assert.Equal(TripStatus.IN_PROGRESS.ToString(), result.Status);
    }

    [Fact]
    public async Task CompleteTrip_WhenValid_ShouldTransitionToCompleted()
    {
        var rider = await SeedRiderAsync();
        var trip = TripEntity.Create(rider.Id, "A", 10.77m, 106.69m, "B", 10.79m, 106.72m, 50000m);
        trip.AcceptDriver(101L);
        trip.DriverArrive();
        trip.StartTrip();
        await _tripRepo.AddAsync(trip);

        var handler = new CompleteTripHandler(_tripRepo);
        var result = await handler.HandleAsync(new CompleteTripCommand(trip.Id));

        Assert.Equal(TripStatus.COMPLETED.ToString(), result.Status);
    }

    [Fact]
    public async Task CancelTrip_WhenSearching_ShouldTransitionToCancelled()
    {
        var rider = await SeedRiderAsync();
        var trip = TripEntity.Create(rider.Id, "A", 10.77m, 106.69m, "B", 10.79m, 106.72m, 50000m);
        await _tripRepo.AddAsync(trip);

        var handler = new CancelTripHandler(_tripRepo);
        var result = await handler.HandleAsync(new CancelTripCommand(trip.Id, CancelledBy.RIDER, "Wait too long"));

        Assert.Equal(TripStatus.CANCELLED.ToString(), result.Status);
        Assert.Equal(CancelledBy.RIDER.ToString(), result.CancelledBy);
        Assert.Equal("Wait too long", result.CancelReason);
    }

    [Fact]
    public async Task CancelTrip_WhenCompleted_ShouldThrowAlreadyCompleted()
    {
        var rider = await SeedRiderAsync();
        var trip = TripEntity.Create(rider.Id, "A", 10.77m, 106.69m, "B", 10.79m, 106.72m, 50000m);
        trip.AcceptDriver(101L);
        trip.DriverArrive();
        trip.StartTrip();
        trip.CompleteTrip();
        await _tripRepo.AddAsync(trip);

        var handler = new CancelTripHandler(_tripRepo);
        var ex = await Assert.ThrowsAsync<AppException>(() =>
            handler.HandleAsync(new CancelTripCommand(trip.Id, CancelledBy.RIDER)));

        Assert.Equal(TripErrors.AlreadyCompleted.Code, ex.Error.Code);
    }

    [Fact]
    public async Task CancelTrip_WhenAlreadyCancelled_ShouldThrowAlreadyCancelled()
    {
        var rider = await SeedRiderAsync();
        var trip = TripEntity.Create(rider.Id, "A", 10.77m, 106.69m, "B", 10.79m, 106.72m, 50000m);
        trip.Cancel(CancelledBy.RIDER);
        await _tripRepo.AddAsync(trip);

        var handler = new CancelTripHandler(_tripRepo);
        var ex = await Assert.ThrowsAsync<AppException>(() =>
            handler.HandleAsync(new CancelTripCommand(trip.Id, CancelledBy.DRIVER)));

        Assert.Equal(TripErrors.AlreadyCancelled.Code, ex.Error.Code);
    }

    [Fact]
    public async Task MarkNoDriverFound_WhenSearching_ShouldTransitionToNoDriverFound()
    {
        var rider = await SeedRiderAsync();
        var trip = TripEntity.Create(rider.Id, "A", 10.77m, 106.69m, "B", 10.79m, 106.72m, 50000m);
        await _tripRepo.AddAsync(trip);

        var handler = new MarkNoDriverFoundHandler(_tripRepo);
        var result = await handler.HandleAsync(new MarkNoDriverFoundCommand(trip.Id));

        Assert.Equal(TripStatus.NO_DRIVER_FOUND.ToString(), result.Status);
    }

    [Fact]
    public async Task GetTripById_WhenExists_ShouldReturnResult()
    {
        var rider = await SeedRiderAsync();
        var trip = TripEntity.Create(rider.Id, "A", 10.77m, 106.69m, "B", 10.79m, 106.72m, 50000m);
        await _tripRepo.AddAsync(trip);

        var handler = new GetTripByIdHandler(_tripRepo);
        var result = await handler.HandleAsync(new GetTripByIdQuery(trip.Id));

        Assert.Equal(trip.Id, result.Id);
        Assert.Equal(rider.Id, result.RiderId);
        Assert.Equal(50000m, result.Cost);
    }

    [Fact]
    public async Task GetTripsByRiderId_ShouldReturnTrips()
    {
        var rider = await SeedRiderAsync();
        await _tripRepo.AddAsync(TripEntity.Create(rider.Id, "A", 10.77m, 106.69m, "B", 10.79m, 106.72m, 50000m));
        await _tripRepo.AddAsync(TripEntity.Create(rider.Id, "C", 10.77m, 106.69m, "D", 10.79m, 106.72m, 70000m));

        var handler = new GetTripsByRiderIdHandler(_tripRepo);
        var result = await handler.HandleAsync(new GetTripsByRiderIdQuery(rider.Id));

        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task GetTripsByDriverId_ShouldReturnTrips()
    {
        var rider = await SeedRiderAsync();
        var trip = TripEntity.Create(rider.Id, "A", 10.77m, 106.69m, "B", 10.79m, 106.72m, 50000m);
        trip.AcceptDriver(42L);
        await _tripRepo.AddAsync(trip);

        var handler = new GetTripsByDriverIdHandler(_tripRepo);
        var result = await handler.HandleAsync(new GetTripsByDriverIdQuery(42L));

        Assert.Single(result.Items);
        Assert.Equal(42L, result.Items[0].DriverId);
    }

    [Fact]
    public async Task GetTripsByStatus_ShouldReturnMatchingTrips()
    {
        var rider = await SeedRiderAsync();
        await _tripRepo.AddAsync(TripEntity.Create(rider.Id, "A", 10.77m, 106.69m, "B", 10.79m, 106.72m, 50000m));

        var handler = new GetTripsByStatusHandler(_tripRepo);
        var result = await handler.HandleAsync(new GetTripsByStatusQuery(TripStatus.SEARCHING_DRIVER));

        Assert.Single(result.Items);
    }
}
