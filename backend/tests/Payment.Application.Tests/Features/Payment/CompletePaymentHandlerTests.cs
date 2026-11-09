using BuildingBlocks.SharedKernel.Exceptions;
using Payment.Application.Features.Payment.Commands.CompletePayment;
using Payment.Domain.Enumerators;
using Payment.Domain.Errors;
using Xunit;
using PaymentEntity = Payment.Domain.Entities.Payment;

namespace Payment.Application.Tests.Features.Payment;

public sealed class CompletePaymentHandlerTests
{
    [Fact]
    public async Task Complete_ExistingPendingPayment_ShouldUpdateStatusToSucceededAndSave()
    {
        // Arrange
        var repository = new FakePaymentRepository();
        var payment = PaymentEntity.Create(
            tripId: 100,
            paymentOptionId: 1,
            amount: 50000m,
            idempotencyKey: "test-idem-key"
        );
        await repository.AddAsync(payment);

        var handler = new CompletePaymentHandler(repository);
        var command = new CompletePaymentCommand(
            PaymentId: payment.Id,
            ProviderTransactionId: "txn-12345",
            GatewayToken: "tok-67890",
            ResolvedProviderId: 2
        );

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.Equal(payment.Id, result.PaymentId);
        Assert.Equal(PaymentStatus.Succeeded.ToString(), result.Status);
        Assert.Equal("txn-12345", result.ProviderTransactionId);
        Assert.Equal("tok-67890", result.GatewayToken);
        Assert.NotNull(result.PaidAt);
        Assert.True(repository.Saved);
    }

    [Fact]
    public async Task Complete_NonExistingPayment_ShouldThrowNotFound()
    {
        // Arrange
        var repository = new FakePaymentRepository();
        var handler = new CompletePaymentHandler(repository);
        var command = new CompletePaymentCommand(
            PaymentId: 999,
            ProviderTransactionId: "txn-12345"
        );

        // Act + Assert
        var exception = await Assert.ThrowsAsync<AppException>(
            () => handler.HandleAsync(command)
        );

        Assert.Equal(PaymentErrors.NotFound.Code, exception.Error.Code);
        Assert.False(repository.Saved);
    }

    [Fact]
    public async Task Complete_EmptyTransactionId_ShouldThrowArgumentException()
    {
        // Arrange
        var repository = new FakePaymentRepository();
        var payment = PaymentEntity.Create(
            tripId: 100,
            paymentOptionId: 1,
            amount: 50000m
        );
        await repository.AddAsync(payment);

        var handler = new CompletePaymentHandler(repository);
        var command = new CompletePaymentCommand(
            PaymentId: payment.Id,
            ProviderTransactionId: ""
        );

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => handler.HandleAsync(command)
        );

        Assert.False(repository.Saved);
    }
}

