using Trip.Application.Abstractions;

namespace Trip.Infrastructure.Payments;

public sealed class FakePaymentGateway : IPaymentGateway
{
    public Task RequestPaymentAsync(
        PaymentRequest request,
        CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
