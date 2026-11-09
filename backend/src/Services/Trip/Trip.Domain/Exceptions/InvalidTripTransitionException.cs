namespace Trip.Domain.Exceptions;

public sealed class InvalidTripTransitionException : DomainException
{
    public InvalidTripTransitionException(string message) : base(message)
    {
    }
}
