using BuildingBlocks.SharedKernel.Exceptions;
using Payment.Application.Abstractions.Data;
using Payment.Application.Abstractions.Email;
using Payment.Application.Features.Payment.Commands.CreatePayment;
using Payment.Domain.Errors;
using Xunit;
using PaymentEntity = Payment.Domain.Entities.Payment;

namespace Payment.Application.Tests.Features.Payment;

public sealed class FakePaymentRepository : IPaymentRepository
{
    private long _nextId = 1;
    public List<PaymentEntity> Payments { get; } = [];
    public bool Saved { get; private set; }

    public Task<PaymentEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Payments.FirstOrDefault(x => x.Id == id));
    }

    public Task<PaymentEntity?> GetByTripIdAsync(long tripId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Payments.FirstOrDefault(x => x.TripId == tripId));
    }

    public Task<PaymentEntity?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Payments.FirstOrDefault(x => x.IdempotencyKey == idempotencyKey));
    }

    public Task<bool> ExistsByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Payments.Any(x => x.IdempotencyKey == idempotencyKey));
    }

    public Task AddAsync(PaymentEntity payment, CancellationToken cancellationToken = default)
    {
        if (payment.Id == 0)
        {
            typeof(PaymentEntity).GetProperty("Id")?.SetValue(payment, _nextId++);
        }

        Payments.Add(payment);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        Saved = true;
        return Task.CompletedTask;
    }
}

public sealed class FakeEmailSender : IEmailSender
{
    public bool ShouldSucceed { get; set; } = true;
    public bool WasCalled { get; private set; }
    public List<string> Recipients { get; } = [];

    public Task<bool> TrySendWelcomeEmailAsync(
        string email,
        string name,
        CancellationToken cancellationToken = default)
    {
        WasCalled = true;
        Recipients.Add(email);
        return Task.FromResult(ShouldSucceed);
    }
}

public sealed class CreatePaymentHandlerTests
{
    [Fact]
    public async Task Create_ValidPayment_ShouldSavePayment()
    {
        // Arrange
        var repository = new FakePaymentRepository();
        var handler = new CreatePaymentHandler(repository);
        var command = new CreatePaymentCommand(
            TripId: 100,
            PaymentOptionId: 1,
            Amount: 50000m,
            IdempotencyKey: "test-idem-key-1",
            ProviderId: 1
        );

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.Id > 0);
        Assert.Equal(100, result.TripId);
        Assert.Equal(1, result.PaymentOptionId);
        Assert.Equal(50000m, result.Amount);
        Assert.Equal("Pending", result.Status);
        Assert.Equal("test-idem-key-1", result.IdempotencyKey);
        Assert.Single(repository.Payments);
        Assert.True(repository.Saved);
    }

    [Fact]
    public async Task Create_DuplicateIdempotencyKey_ShouldThrow()
    {
        // Arrange
        var repository = new FakePaymentRepository();
        var existingPayment = PaymentEntity.Create(
            tripId: 100,
            paymentOptionId: 1,
            amount: 50000m,
            idempotencyKey: "dup-idem-key"
        );
        await repository.AddAsync(existingPayment);

        var handler = new CreatePaymentHandler(repository);
        var command = new CreatePaymentCommand(
            TripId: 101,
            PaymentOptionId: 1,
            Amount: 75000m,
            IdempotencyKey: "dup-idem-key"
        );

        // Act + Assert
        var exception = await Assert.ThrowsAsync<AppException>(
            () => handler.HandleAsync(command)
        );

        Assert.Equal(PaymentErrors.DuplicateIdempotencyKey.Code, exception.Error.Code);
        Assert.Single(repository.Payments);
        Assert.False(repository.Saved);
    }

    [Fact]
    public async Task Create_InvalidAmount_ShouldThrowArgumentException()
    {
        // Arrange
        var repository = new FakePaymentRepository();
        var handler = new CreatePaymentHandler(repository);
        var command = new CreatePaymentCommand(
            TripId: 100,
            PaymentOptionId: 1,
            Amount: -5000m
        );

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => handler.HandleAsync(command)
        );

        Assert.Empty(repository.Payments);
        Assert.False(repository.Saved);
    }

    [Fact]
    public async Task Create_InvalidTripId_ShouldThrowArgumentException()
    {
        // Arrange
        var repository = new FakePaymentRepository();
        var handler = new CreatePaymentHandler(repository);
        var command = new CreatePaymentCommand(
            TripId: 0,
            PaymentOptionId: 1,
            Amount: 50000m
        );

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => handler.HandleAsync(command)
        );

        Assert.Empty(repository.Payments);
        Assert.False(repository.Saved);
    }

    [Fact]
    public async Task Create_WithoutIdempotencyKey_ShouldSaveSuccessfully()
    {
        // Arrange
        var repository = new FakePaymentRepository();
        var handler = new CreatePaymentHandler(repository);
        var command = new CreatePaymentCommand(
            TripId: 100,
            PaymentOptionId: 1,
            Amount: 50000m,
            IdempotencyKey: null
        );

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.Id > 0);
        Assert.Null(result.IdempotencyKey);
        Assert.Single(repository.Payments);
        Assert.True(repository.Saved);
    }
}

