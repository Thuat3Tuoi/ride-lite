namespace Trip.Domain.Enums;

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
