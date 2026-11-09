namespace Trip.Domain.Enumerators;

public enum TripStatus
{
    Requested,
    Quoted,
    AwaitingDriver,
    EnRoute,
    DriverArrived,
    PickedUp,
    DroppedOff,
    PaymentPending,
    Paid,
    Cancelled,
    Failed
}
