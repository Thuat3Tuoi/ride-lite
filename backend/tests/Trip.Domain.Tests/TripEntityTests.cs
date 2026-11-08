using Trip.Domain.Entities;
using Trip.Domain.Enums;
using Trip.Domain.Events;
using Trip.Domain.ValueObjects;

namespace Trip.Domain.Tests;

public class TripEntityTests
{
    [Fact]
    public void NewTrip_StartsInRequestedStatus()
    {
        var trip = TripTestFactory.NewTrip();

        Assert.Equal(TripStatus.Requested, trip.Status);
        Assert.Equal(1, trip.RiderId);
        Assert.Null(trip.DriverId);
        Assert.Null(trip.Cost);
        Assert.Equal(0, trip.DriverRejectCount);
        Assert.NotEqual(default, trip.CreatedAt);
        Assert.Equal(trip.CreatedAt, trip.UpdatedAt);
    }

    [Fact]
    public void Quote_FromRequested_RecordsCostAndDriver()
    {
        var trip = TripTestFactory.NewTrip();

        trip.Quote(new Money(85000m), 501);

        Assert.Equal(TripStatus.Quoted, trip.Status);
        Assert.Equal(85000m, trip.Cost);
        Assert.Equal(501, trip.DriverId);
    }

    [Fact]
    public void ConfirmQuote_MovesToAwaitingDriver()
    {
        var trip = TripTestFactory.Quoted();

        trip.ConfirmQuote();

        Assert.Equal(TripStatus.AwaitingDriver, trip.Status);
    }

    [Fact]
    public void RejectQuote_CancelsTripByRider()
    {
        var trip = TripTestFactory.Quoted();

        trip.RejectQuote("Too expensive");

        Assert.Equal(TripStatus.Cancelled, trip.Status);
        Assert.Equal(CancelledBy.Rider, trip.CancelledBy);
        Assert.Equal("Too expensive", trip.CancelReason);
        Assert.NotNull(trip.CancelledAt);

        var domainEvent = Assert.Single(trip.DomainEvents.OfType<TripCancelled>());
        Assert.Equal(CancelledBy.Rider, domainEvent.CancelledBy);
    }

    [Fact]
    public void DriverAccept_MovesToEnRoute_AndRaisesTripAccepted()
    {
        var trip = TripTestFactory.AwaitingDriver();

        trip.DriverAccept(777);

        Assert.Equal(TripStatus.EnRoute, trip.Status);
        Assert.Equal(777, trip.DriverId);
        Assert.NotNull(trip.AcceptedAt);

        var domainEvent = Assert.Single(trip.DomainEvents.OfType<TripAccepted>());
        Assert.Equal(777, domainEvent.DriverId);
    }

    [Fact]
    public void DriverReject_UpToThreshold_StaysAwaitingDriver()
    {
        var trip = TripTestFactory.AwaitingDriver();

        for (var i = 0; i < TripEntity.MaxDriverRejectCount; i++)
        {
            trip.DriverReject(501);
        }

        Assert.Equal(TripStatus.AwaitingDriver, trip.Status);
        Assert.Equal(TripEntity.MaxDriverRejectCount, trip.DriverRejectCount);
        Assert.Null(trip.DriverId);
        Assert.Empty(trip.DomainEvents.OfType<TripFailed>());
    }

    [Fact]
    public void DriverReject_AboveThreshold_FailsTripBySystem()
    {
        var trip = TripTestFactory.AwaitingDriver();

        for (var i = 0; i <= TripEntity.MaxDriverRejectCount; i++)
        {
            trip.DriverReject(501);
        }

        Assert.Equal(TripStatus.Failed, trip.Status);
        Assert.Equal(CancelledBy.System, trip.CancelledBy);
        Assert.NotNull(trip.CancelledAt);
        Assert.Single(trip.DomainEvents.OfType<TripFailed>());
    }

    [Fact]
    public void Arrive_MovesToDriverArrived_AndRecordsTimestamp()
    {
        var trip = TripTestFactory.EnRoute();

        trip.Arrive();

        Assert.Equal(TripStatus.DriverArrived, trip.Status);
        Assert.NotNull(trip.DriverArrivedAt);
    }

    [Fact]
    public void Pickup_MovesToPickedUp_AndRecordsTimestamp()
    {
        var trip = TripTestFactory.DriverArrived();

        trip.Pickup();

        Assert.Equal(TripStatus.PickedUp, trip.Status);
        Assert.NotNull(trip.StartedAt);
    }

    [Fact]
    public void Dropoff_MovesToPaymentPending_AndRaisesRideCompleted()
    {
        var trip = TripTestFactory.PickedUp();
        var tripId = trip.Id;

        trip.Dropoff();

        Assert.Equal(TripStatus.PaymentPending, trip.Status);
        Assert.NotNull(trip.CompletedAt);

        var domainEvent = Assert.Single(trip.DomainEvents.OfType<RideCompleted>());
        Assert.Equal(tripId, domainEvent.TripId);
        Assert.Equal(85000m, domainEvent.Amount);
        Assert.Equal($"trip-{tripId}-payment", domainEvent.IdempotencyKey);
    }

    [Fact]
    public void PaymentSettled_MovesToPaid()
    {
        var trip = TripTestFactory.PaymentPending();

        trip.PaymentSettled();

        Assert.Equal(TripStatus.Paid, trip.Status);
    }

    [Fact]
    public void PaymentFailed_KeepsTripInPaymentPending()
    {
        var trip = TripTestFactory.PaymentPending();

        trip.PaymentFailed();

        Assert.Equal(TripStatus.PaymentPending, trip.Status);
    }

    [Fact]
    public void InvalidTransition_ThrowsDomainException()
    {
        var trip = TripTestFactory.NewTrip();

        Assert.Throws<Trip.Domain.Exceptions.DomainException>(() => trip.Pickup());
    }

    [Fact]
    public void Paid_IsFinal()
    {
        var trip = TripTestFactory.PaymentPending();
        trip.PaymentSettled();

        Assert.Throws<Trip.Domain.Exceptions.DomainException>(() => trip.PaymentFailed());
        Assert.Throws<Trip.Domain.Exceptions.DomainException>(() => trip.Cancel(CancelledBy.Rider));
        Assert.Throws<Trip.Domain.Exceptions.DomainException>(() => trip.DriverAccept(1));
    }

    [Fact]
    public void Cancelled_IsFinal()
    {
        var trip = TripTestFactory.Quoted();
        trip.RejectQuote();

        Assert.Throws<Trip.Domain.Exceptions.DomainException>(() => trip.ConfirmQuote());
        Assert.Throws<Trip.Domain.Exceptions.DomainException>(() => trip.DriverAccept(1));
    }

    [Fact]
    public void DequeueDomainEvents_ClearsCollectedEvents()
    {
        var trip = TripTestFactory.Quoted();
        trip.RejectQuote();

        var events = trip.DequeueDomainEvents();

        Assert.Single(events);
        Assert.Empty(trip.DomainEvents);
    }
}
