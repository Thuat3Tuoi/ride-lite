using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Payment.Infrastructure.Data.DataContext;
using Payment.Integration.Tests.Infrastructure;
using Payment.Presentation.Contracts.Requests;
using Payment.Presentation.Contracts.Responses;
using Xunit;

namespace Payment.Integration.Tests.Controllers;

public sealed class PaymentMethodsControllerTests
{
    [Fact]
    public async Task CreatePaymentMethod_ValidRequest_Returns201()
    {
        // Arrange
        using var factory = new PaymentApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var request = new CreatePaymentMethodRequest
        {
            Code = "E_WALLET",
            Name = "Ví điện tử",
            Active = true
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/payment-methods", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CreatePaymentMethodResponse>();
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("E_WALLET", result.Code);
        Assert.Equal("Ví điện tử", result.Name);
        Assert.True(result.Active);

        // Assert DB
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var entity = await db.PaymentMethods.FirstOrDefaultAsync(m => m.Code == "E_WALLET");
        Assert.NotNull(entity);
    }

    [Fact]
    public async Task CreatePaymentMethod_DuplicateCode_Returns409()
    {
        // Arrange
        using var factory = new PaymentApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        // CASH already seeded in InitializeDatabaseAsync
        var request = new CreatePaymentMethodRequest
        {
            Code = "CASH",
            Name = "Trùng Cash",
            Active = true
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/payment-methods", request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task GetActivePaymentMethods_Returns200WithList()
    {
        // Arrange
        using var factory = new PaymentApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        // Act
        var response = await client.GetAsync("/api/payment-methods/active");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var list = await response.Content.ReadFromJsonAsync<List<PaymentMethodResponse>>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
        Assert.Contains(list, m => m.Code == "CASH");
    }

    [Fact]
    public async Task DeactivateAndActivate_ExistingMethod_UpdatesStatus()
    {
        // Arrange
        using var factory = new PaymentApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var cashMethod = await db.PaymentMethods.FirstAsync(m => m.Code == "CASH");

        // Deactivate
        var deactResponse = await client.PutAsync($"/api/payment-methods/{cashMethod.Id}/deactivate", null);
        Assert.Equal(HttpStatusCode.OK, deactResponse.StatusCode);
        var deactResult = await deactResponse.Content.ReadFromJsonAsync<PaymentMethodStatusResponse>();
        Assert.NotNull(deactResult);
        Assert.False(deactResult.Active);

        // Activate again
        var actResponse = await client.PutAsync($"/api/payment-methods/{cashMethod.Id}/activate", null);
        Assert.Equal(HttpStatusCode.OK, actResponse.StatusCode);
        var actResult = await actResponse.Content.ReadFromJsonAsync<PaymentMethodStatusResponse>();
        Assert.NotNull(actResult);
        Assert.True(actResult.Active);
    }
}

