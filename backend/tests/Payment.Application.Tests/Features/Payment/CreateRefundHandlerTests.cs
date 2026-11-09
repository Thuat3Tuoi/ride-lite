using BuildingBlocks.SharedKernel.Exceptions;
using Payment.Application.Abstractions.Data;
using Payment.Application.Features.Refund.Commands.CreateRefund;
using Payment.Domain.Entities;
using Payment.Domain.Errors;
using Xunit;

namespace Payment.Application.Tests.Features.Payment;

public sealed class FakeRefundRepository : IRefundRepository
{
    private long _nextId = 1;
    public List<Refund> Refunds { get; } = [];
    public bool Saved { get; private set; }

    public Task<Refund?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Refunds.FirstOrDefault(r => r.Id == id));
    }

    public Task<IReadOnlyList<Refund>> GetByPaymentIdAsync(long paymentId, CancellationToken cancellationToken = default)
    {
        var list = Refunds.Where(r => r.PaymentId == paymentId).ToList();
        return Task.FromResult<IReadOnlyList<Refund>>(list);
    }

    public Task<Refund?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Refunds.FirstOrDefault(r => r.IdempotencyKey == idempotencyKey));
    }

    public Task<bool> ExistsByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Refunds.Any(r => r.IdempotencyKey == idempotencyKey));
    }

    public Task AddAsync(Refund refund, CancellationToken cancellationToken = default)
    {
        if (refund.Id == 0)
        {
            typeof(Refund).GetProperty("Id")?.SetValue(refund, _nextId++);
        }
        Refunds.Add(refund);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        Saved = true;
        return Task.CompletedTask;
    }
}

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

