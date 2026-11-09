using Trip.Application.Abstractions.Services;

namespace Trip.Infrastructure.Services;

public sealed class FakePaymentGateway : IPaymentGateway
{
    public Task RequestPaymentAsync(
        PaymentRequest request,
        CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
