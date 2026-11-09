using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Payment.Domain.Enumerators;
using Payment.Infrastructure.Data.DataContext;
using Payment.Integration.Tests.Infrastructure;
using Payment.Presentation.Contracts.Requests;
using Payment.Presentation.Contracts.Responses;
using Xunit;

namespace Payment.Integration.Tests.Controllers;

public sealed class PaymentsControllerTests
{
    [Fact]
    public async Task CreatePayment_ValidRequest_Returns201()
    {
        // Arrange
        using var factory = new PaymentApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var request = new CreatePaymentRequest
        {
            TripId = 100,
            PaymentOptionId = 1,
            Amount = 50000m,
            IdempotencyKey = "idem-test-201"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/payments", request);

        // Assert HTTP response
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CreatePaymentResponse>();
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(100, result.TripId);
        Assert.Equal(50000m, result.Amount);
        Assert.Equal("Pending", result.Status);
        Assert.Equal("idem-test-201", result.IdempotencyKey);

        // Assert database thực sự có Payment
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var payment = await db.Payments.SingleAsync();
        Assert.Equal(result.Id, payment.Id);
        Assert.Equal(100, payment.TripId);
        Assert.Equal(50000m, payment.Amount);
        Assert.Equal(PaymentStatus.Pending, payment.Status);
    }

    [Fact]
    public async Task CreatePayment_DuplicateIdempotencyKey_Returns409()
    {
        // Arrange
        using var factory = new PaymentApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var request = new CreatePaymentRequest
        {
            TripId = 100,
            PaymentOptionId = 1,
            Amount = 50000m,
            IdempotencyKey = "dup-idem-key-int"
        };

        // Tạo lần đầu
        var firstResponse = await client.PostAsJsonAsync("/api/payments", request);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        // Tạo lại bằng cùng idempotency key
        var secondResponse = await client.PostAsJsonAsync("/api/payments", request);
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);

        // Database vẫn chỉ có 1 Payment
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Assert.Equal(1, await db.Payments.CountAsync());
    }

    [Fact]
    public async Task CreatePayment_InvalidAmount_Returns400()
    {
        // Arrange
        using var factory = new PaymentApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var request = new
        {
            TripId = 100,
            PaymentOptionId = 1,
            Amount = -5000m,
            IdempotencyKey = "invalid-amount"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/payments", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        // Database không tạo record
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Assert.Equal(0, await db.Payments.CountAsync());
    }

    [Fact]
    public async Task CreatePayment_MissingTripId_Returns400()
    {
        // Arrange
        using var factory = new PaymentApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var request = new
        {
            PaymentOptionId = 1,
            Amount = 50000m
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/payments", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Assert.Equal(0, await db.Payments.CountAsync());
    }

    [Fact]
    public async Task GetPaymentById_ExistingPayment_Returns200()
    {
        // Arrange
        using var factory = new PaymentApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var createRequest = new CreatePaymentRequest
        {
            TripId = 101,
            PaymentOptionId = 1,
            Amount = 60000m,
            IdempotencyKey = "get-by-id-key"
        };

        var createResponse = await client.PostAsJsonAsync("/api/payments", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<CreatePaymentResponse>();
        Assert.NotNull(created);

        // Act
        var getResponse = await client.GetAsync($"/api/payments/{created.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var payment = await getResponse.Content.ReadFromJsonAsync<PaymentResponse>();
        Assert.NotNull(payment);
        Assert.Equal(created.Id, payment.Id);
        Assert.Equal(101, payment.TripId);
        Assert.Equal(60000m, payment.Amount);
    }

    [Fact]
    public async Task GetPaymentById_NonExisting_Returns404()
    {
        // Arrange
        using var factory = new PaymentApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        // Act
        var response = await client.GetAsync("/api/payments/9999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ProcessPayment_ExistingPayment_Returns200AndUpdatesStatus()
    {
        // Arrange
        using var factory = new PaymentApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var createRequest = new CreatePaymentRequest
        {
            TripId = 102,
            PaymentOptionId = 1,
            Amount = 45000m,
            IdempotencyKey = "process-test-key"
        };

        var createResponse = await client.PostAsJsonAsync("/api/payments", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<CreatePaymentResponse>();
        Assert.NotNull(created);

        // Act
        var processResponse = await client.PostAsync($"/api/payments/{created.Id}/process", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, processResponse.StatusCode);
        var processResult = await processResponse.Content.ReadFromJsonAsync<ProcessPaymentResponse>();
        Assert.NotNull(processResult);
        Assert.Equal(created.Id, processResult.PaymentId);
        Assert.Equal(PaymentStatus.Processing.ToString(), processResult.Status);

        // Database verified
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var payment = await db.Payments.FindAsync(created.Id);
        Assert.NotNull(payment);
        Assert.Equal(PaymentStatus.Processing, payment.Status);
    }

    [Fact]
    public async Task CompletePayment_ExistingPayment_Returns200AndSetsSucceeded()
    {
        // Arrange
        using var factory = new PaymentApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var createRequest = new CreatePaymentRequest
        {
            TripId = 103,
            PaymentOptionId = 1,
            Amount = 80000m,
            IdempotencyKey = "complete-test-key"
        };

        var createResponse = await client.PostAsJsonAsync("/api/payments", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<CreatePaymentResponse>();
        Assert.NotNull(created);

        var completeRequest = new CompletePaymentRequest
        {
            ProviderTransactionId = "TXN_VNPOST_001",
            GatewayToken = "GW_TOK_ABC",
            ResolvedProviderId = 1
        };

        // Act
        var completeResponse = await client.PostAsJsonAsync($"/api/payments/{created.Id}/complete", completeRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, completeResponse.StatusCode);
        var completeResult = await completeResponse.Content.ReadFromJsonAsync<CompletePaymentResponse>();
        Assert.NotNull(completeResult);
        Assert.Equal(created.Id, completeResult.PaymentId);
        Assert.Equal(PaymentStatus.Succeeded.ToString(), completeResult.Status);
        Assert.Equal("TXN_VNPOST_001", completeResult.ProviderTransactionId);

        // Database verified
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var payment = await db.Payments.FindAsync(created.Id);
        Assert.NotNull(payment);
        Assert.Equal(PaymentStatus.Succeeded, payment.Status);
        Assert.Equal("TXN_VNPOST_001", payment.ProviderTransactionId);
        Assert.NotNull(payment.PaidAt);
    }

    [Fact]
    public async Task CancelPayment_ExistingPayment_Returns200AndSetsCanceled()
    {
        // Arrange
        using var factory = new PaymentApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var createRequest = new CreatePaymentRequest
        {
            TripId = 104,
            PaymentOptionId = 1,
            Amount = 30000m,
            IdempotencyKey = "cancel-test-key"
        };

        var createResponse = await client.PostAsJsonAsync("/api/payments", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<CreatePaymentResponse>();
        Assert.NotNull(created);

        // Act
        var cancelResponse = await client.PostAsync($"/api/payments/{created.Id}/cancel", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, cancelResponse.StatusCode);
        var cancelResult = await cancelResponse.Content.ReadFromJsonAsync<CancelPaymentResponse>();
        Assert.NotNull(cancelResult);
        Assert.Equal(created.Id, cancelResult.PaymentId);
        Assert.Equal(PaymentStatus.Canceled.ToString(), cancelResult.Status);

        // Database verified
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var payment = await db.Payments.FindAsync(created.Id);
        Assert.NotNull(payment);
        Assert.Equal(PaymentStatus.Canceled, payment.Status);
    }
}

