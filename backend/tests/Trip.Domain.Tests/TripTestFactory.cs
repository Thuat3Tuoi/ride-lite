using Trip.Domain.Entities;
using Trip.Domain.ValueObjects;

namespace Trip.Domain.Tests;

internal static class TripTestFactory
{
    public static TripEntity NewTrip(long riderId = 1) =>
        new(
            riderId,
            "1 Nguyen Trai, District 1",
            new GeoPoint(10.7769m, 106.7009m),
            "1 Vo Van Kiet, District 1",
            new GeoPoint(10.7720m, 106.7050m));

    public static TripEntity Quoted()
    {
        var trip = NewTrip();
        trip.Quote(new Money(85000m), 501);
        return trip;
    }

    public static TripEntity AwaitingDriver()
    {
        var trip = Quoted();
        trip.ConfirmQuote();
        return trip;
    }

    public static TripEntity EnRoute()
    {
        var trip = AwaitingDriver();
        trip.DriverAccept(501);
        return trip;
    }

    public static TripEntity DriverArrived()
    {
        var trip = EnRoute();
        trip.Arrive();
        return trip;
    }

    public static TripEntity PickedUp()
    {
        var trip = DriverArrived();
        trip.Pickup();
        return trip;
    }

    public static TripEntity PaymentPending()
    {
        var trip = PickedUp();
        trip.Dropoff();
        return trip;
    }
}
