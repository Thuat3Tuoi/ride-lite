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

public sealed class RefundsControllerTests
{
    private static async Task<long> CreateTestPaymentAsync(HttpClient client)
    {
        var paymentRequest = new CreatePaymentRequest
        {
            TripId = 200,
            PaymentOptionId = 1,
            Amount = 100000m,
            IdempotencyKey = Guid.NewGuid().ToString()
        };

        var response = await client.PostAsJsonAsync("/api/payments", paymentRequest);
        var result = await response.Content.ReadFromJsonAsync<CreatePaymentResponse>();
        return result!.Id;
    }

    [Fact]
    public async Task CreateRefund_ValidRequest_Returns201()
    {
        // Arrange
        using var factory = new PaymentApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var paymentId = await CreateTestPaymentAsync(client);

        var request = new CreateRefundRequest
        {
            PaymentId = paymentId,
            Amount = 30000m,
            IdempotencyKey = "ref-idem-valid-201",
            Reason = "Trip canceled by driver"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/refunds", request);

        // Assert HTTP response
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CreateRefundResponse>();
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(paymentId, result.PaymentId);
        Assert.Equal(30000m, result.Amount);
        Assert.Equal("Pending", result.Status);
        Assert.Equal("ref-idem-valid-201", result.IdempotencyKey);

        // Assert database
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var refund = await db.Refunds.SingleAsync(r => r.Id == result.Id);
        Assert.Equal(paymentId, refund.PaymentId);
        Assert.Equal(30000m, refund.Amount);
        Assert.Equal(RefundStatus.Pending, refund.Status);
    }

    [Fact]
    public async Task CreateRefund_DuplicateIdempotencyKey_Returns409()
    {
        // Arrange
        using var factory = new PaymentApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var paymentId = await CreateTestPaymentAsync(client);

        var request = new CreateRefundRequest
        {
            PaymentId = paymentId,
            Amount = 20000m,
            IdempotencyKey = "dup-refund-idem-key",
            Reason = "Duplicate test"
        };

        // Tạo lần đầu
        var firstResponse = await client.PostAsJsonAsync("/api/refunds", request);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        // Tạo lại bằng cùng idempotency key
        var secondResponse = await client.PostAsJsonAsync("/api/refunds", request);
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);

        // Database chỉ có 1 record refund
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Assert.Equal(1, await db.Refunds.CountAsync(r => r.IdempotencyKey == "dup-refund-idem-key"));
    }

    [Fact]
    public async Task GetRefundById_ExistingRefund_Returns200()
    {
        // Arrange
        using var factory = new PaymentApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var paymentId = await CreateTestPaymentAsync(client);

        var request = new CreateRefundRequest
        {
            PaymentId = paymentId,
            Amount = 15000m,
            IdempotencyKey = "get-refund-key",
            Reason = "Partial refund"
        };

        var createResponse = await client.PostAsJsonAsync("/api/refunds", request);
        var created = await createResponse.Content.ReadFromJsonAsync<CreateRefundResponse>();
        Assert.NotNull(created);

        // Act
        var getResponse = await client.GetAsync($"/api/refunds/{created.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var refund = await getResponse.Content.ReadFromJsonAsync<RefundResponse>();
        Assert.NotNull(refund);
        Assert.Equal(created.Id, refund.Id);
        Assert.Equal(paymentId, refund.PaymentId);
        Assert.Equal(15000m, refund.Amount);
    }

    [Fact]
    public async Task GetRefundById_NonExisting_Returns404()
    {
        // Arrange
        using var factory = new PaymentApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        // Act
        var response = await client.GetAsync("/api/refunds/9999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

