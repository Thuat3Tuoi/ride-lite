using BuildingBlocks.SharedKernel.Exceptions;
using Payment.Application.Features.Payment.Commands.CancelPayment;
using Payment.Domain.Enumerators;
using Payment.Domain.Errors;
using Xunit;
using PaymentEntity = Payment.Domain.Entities.Payment;

namespace Payment.Application.Tests.Features.Payment;

public sealed class CancelPaymentHandlerTests
{
    [Fact]
    public async Task Cancel_ExistingPendingPayment_ShouldUpdateStatusToCanceled()
    {
        // Arrange
        var repository = new FakePaymentRepository();
        var payment = PaymentEntity.Create(
            tripId: 100,
            paymentOptionId: 1,
            amount: 50000m
        );
        await repository.AddAsync(payment);

        var handler = new CancelPaymentHandler(repository);
        var command = new CancelPaymentCommand(payment.Id);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.Equal(payment.Id, result.PaymentId);
        Assert.Equal(PaymentStatus.Canceled.ToString(), result.Status);
        Assert.True(repository.Saved);
    }

    [Fact]
    public async Task Cancel_NonExistingPayment_ShouldThrowNotFound()
    {
        // Arrange
        var repository = new FakePaymentRepository();
        var handler = new CancelPaymentHandler(repository);
        var command = new CancelPaymentCommand(999);

        // Act + Assert
        var exception = await Assert.ThrowsAsync<AppException>(
            () => handler.HandleAsync(command)
        );

        Assert.Equal(PaymentErrors.NotFound.Code, exception.Error.Code);
        Assert.False(repository.Saved);
    }

    [Fact]
    public async Task Cancel_AlreadySucceededPayment_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var repository = new FakePaymentRepository();
        var payment = PaymentEntity.Create(
            tripId: 100,
            paymentOptionId: 1,
            amount: 50000m
        );
        payment.MarkSucceeded("txn-12345");
        await repository.AddAsync(payment);

        var handler = new CancelPaymentHandler(repository);
        var command = new CancelPaymentCommand(payment.Id);

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.HandleAsync(command)
        );

        Assert.False(repository.Saved);
    }
}

