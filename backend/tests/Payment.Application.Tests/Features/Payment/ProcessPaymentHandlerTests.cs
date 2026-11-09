using BuildingBlocks.SharedKernel.Exceptions;
using Payment.Application.Features.Payment.Commands.ProcessPayment;
using Payment.Domain.Enumerators;
using Payment.Domain.Errors;
using Xunit;
using PaymentEntity = Payment.Domain.Entities.Payment;

namespace Payment.Application.Tests.Features.Payment;

public sealed class ProcessPaymentHandlerTests
{
    [Fact]
    public async Task Process_ExistingPendingPayment_ShouldUpdateStatusToProcessing()
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

        var handler = new ProcessPaymentHandler(repository);
        var command = new ProcessPaymentCommand(payment.Id);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.Equal(payment.Id, result.PaymentId);
        Assert.Equal(PaymentStatus.Processing.ToString(), result.Status);
        Assert.True(repository.Saved);
    }

    [Fact]
    public async Task Process_NonExistingPayment_ShouldThrowNotFound()
    {
        // Arrange
        var repository = new FakePaymentRepository();
        var handler = new ProcessPaymentHandler(repository);
        var command = new ProcessPaymentCommand(999);

        // Act + Assert
        var exception = await Assert.ThrowsAsync<AppException>(
            () => handler.HandleAsync(command)
        );

        Assert.Equal(PaymentErrors.NotFound.Code, exception.Error.Code);
        Assert.False(repository.Saved);
    }

    [Fact]
    public async Task Process_AlreadyProcessingPayment_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var repository = new FakePaymentRepository();
        var payment = PaymentEntity.Create(
            tripId: 100,
            paymentOptionId: 1,
            amount: 50000m
        );
        payment.MarkProcessing();
        await repository.AddAsync(payment);

        var handler = new ProcessPaymentHandler(repository);
        var command = new ProcessPaymentCommand(payment.Id);

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.HandleAsync(command)
        );

        Assert.False(repository.Saved);
    }
}

