namespace Trip.Domain.Constants;

public static class TripRules
{
    public const int MaxDriverRejectCount = 3;

    public static string PaymentIdempotencyKey(long tripId) => $"trip-{tripId}-payment";
}
