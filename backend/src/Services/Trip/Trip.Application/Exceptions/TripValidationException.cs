namespace Trip.Application.Exceptions;

public sealed class TripValidationException : Exception
{
    public TripValidationException(string message) : base(message)
    {
    }
}
