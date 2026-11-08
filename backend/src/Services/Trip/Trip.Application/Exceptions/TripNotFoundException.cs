namespace Trip.Application.Exceptions;

public sealed class TripNotFoundException : Exception
{
    public TripNotFoundException(long tripId)
        : base($"Trip {tripId} was not found.")
    {
        TripId = tripId;
    }

    public long TripId { get; }
}
