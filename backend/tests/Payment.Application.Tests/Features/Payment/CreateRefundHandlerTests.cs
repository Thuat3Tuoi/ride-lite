using BuildingBlocks.SharedKernel.Exceptions;
using Payment.Application.Features.Refund.Commands.CreateRefund;
using Payment.Application.Tests.Fakes;
using Payment.Domain.Entities;
using Payment.Domain.Errors;
using Xunit;

namespace Payment.Application.Tests.Features.Payment;

public sealed class CreateRefundHandlerTests
{
    [Fact]
    public async Task Create_ValidRefund_ShouldSaveRefund()
    {
        // Arrange
        var repository = new FakeRefundRepository();
        var handler = new CreateRefundHandler(repository);
        var command = new CreateRefundCommand(
            PaymentId: 10,
            Amount: 25000m,
            IdempotencyKey: "ref-idem-test-1",
            Reason: "Customer canceled ride"
        );

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.Id > 0);
        Assert.Equal(10, result.PaymentId);
        Assert.Equal(25000m, result.Amount);
        Assert.Equal("Pending", result.Status);
        Assert.Equal("ref-idem-test-1", result.IdempotencyKey);
        Assert.Single(repository.Refunds);
        Assert.True(repository.Saved);
    }

    [Fact]
    public async Task Create_DuplicateIdempotencyKey_ShouldThrow()
    {
        // Arrange
        var repository = new FakeRefundRepository();
        var existing = Refund.Create(
            paymentId: 10,
            amount: 25000m,
            idempotencyKey: "dup-ref-key"
        );
        await repository.AddAsync(existing);

        var handler = new CreateRefundHandler(repository);
        var command = new CreateRefundCommand(
            PaymentId: 10,
            Amount: 25000m,
            IdempotencyKey: "dup-ref-key"
        );

        // Act + Assert
        var exception = await Assert.ThrowsAsync<AppException>(
            () => handler.HandleAsync(command)
        );

        Assert.Equal(PaymentErrors.DuplicateRefundIdempotencyKey.Code, exception.Error.Code);
        Assert.Single(repository.Refunds);
        Assert.False(repository.Saved);
    }

    [Fact]
    public async Task Create_InvalidAmount_ShouldThrowArgumentException()
    {
        // Arrange
        var repository = new FakeRefundRepository();
        var handler = new CreateRefundHandler(repository);
        var command = new CreateRefundCommand(
            PaymentId: 10,
            Amount: -100m,
            IdempotencyKey: "ref-invalid-amount"
        );

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => handler.HandleAsync(command)
        );

        Assert.Empty(repository.Refunds);
        Assert.False(repository.Saved);
    }
}

