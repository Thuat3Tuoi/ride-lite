namespace Trip.Application.Abstractions;

public sealed record PaymentRequest(long TripId, long RiderId, decimal Amount, string IdempotencyKey);

public interface IPaymentGateway
{
    Task RequestPaymentAsync(PaymentRequest request, CancellationToken cancellationToken = default);
}
