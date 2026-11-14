using System.Net;
using System.Net.Http.Json;
using MatchingPricing.Domain.Enumerators;
using MatchingPricing.Integration.Tests.Infrastructure;
using MatchingPricing.Presentation.Contracts.Requests;
using MatchingPricing.Presentation.Contracts.Responses;
using Xunit;

namespace MatchingPricing.Integration.Tests.Controllers;

public sealed class AssignmentsControllerTests
{
    private static async Task<long> CreateAvailableDriverAsync(HttpClient client, string phone)
    {
        var response = await client.PostAsJsonAsync("/api/drivers",
            new CreateDriverRequest("Test Driver", phone, null, DriverStatus.available));
        var driver = await response.Content.ReadFromJsonAsync<DriverResponse>();

        await client.PostAsJsonAsync($"/api/drivers/{driver!.Id}/status",
            new UpdateDriverStatusRequest(DriverStatus.available));

        return driver.Id;
    }

    [Fact]
    public async Task CreateAssignment_ThenAccept_SetsStatusAcceptedAndDriverBusy()
    {
        using var factory = new MatchingPricingApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var driverId = await CreateAvailableDriverAsync(client, "0908000001");

        // 1. Create assignment
        var createResponse = await client.PostAsJsonAsync("/api/assignments",
            new CreateAssignmentRequest(2001L, driverId, TimeSpan.FromMinutes(2)));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var assignment = await createResponse.Content.ReadFromJsonAsync<DriverAssignmentResponse>();
        Assert.NotNull(assignment);
        Assert.Equal("OFFERED", assignment.Status);

        // 2. Accept assignment
        var acceptResponse = await client.PostAsJsonAsync($"/api/assignments/{assignment.Id}/accept",
            new AcceptAssignmentRequest(driverId));
        Assert.Equal(HttpStatusCode.OK, acceptResponse.StatusCode);

        // 3. Verify driver is now busy
        var driverResponse = await client.GetAsync($"/api/drivers/{driverId}");
        var driver = await driverResponse.Content.ReadFromJsonAsync<DriverResponse>();
        Assert.NotNull(driver);
        Assert.Equal("busy", driver.Status);
    }

    [Fact]
    public async Task CreateAssignment_ThenReject_SetsStatusRejected()
    {
        using var factory = new MatchingPricingApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var driverId = await CreateAvailableDriverAsync(client, "0908000002");

        var createResponse = await client.PostAsJsonAsync("/api/assignments",
            new CreateAssignmentRequest(2002L, driverId, TimeSpan.FromMinutes(2)));
        var assignment = await createResponse.Content.ReadFromJsonAsync<DriverAssignmentResponse>();

        var rejectResponse = await client.PostAsJsonAsync($"/api/assignments/{assignment!.Id}/reject",
            new RejectAssignmentRequest(driverId));
        Assert.Equal(HttpStatusCode.OK, rejectResponse.StatusCode);
    }

    [Fact]
    public async Task MultipleDrivers_FirstAcceptWins_SecondAcceptThrowsConflict()
    {
        using var factory = new MatchingPricingApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var driver1Id = await CreateAvailableDriverAsync(client, "0908000003");
        var driver2Id = await CreateAvailableDriverAsync(client, "0908000004");
        const long tripId = 3001L;

        // Offer to both drivers
        var offer1 = await client.PostAsJsonAsync("/api/assignments",
            new CreateAssignmentRequest(tripId, driver1Id, TimeSpan.FromMinutes(2)));
        var assignment1 = await offer1.Content.ReadFromJsonAsync<DriverAssignmentResponse>();

        var offer2 = await client.PostAsJsonAsync("/api/assignments",
            new CreateAssignmentRequest(tripId, driver2Id, TimeSpan.FromMinutes(2)));
        var assignment2 = await offer2.Content.ReadFromJsonAsync<DriverAssignmentResponse>();

        // Driver 1 accepts
        var accept1 = await client.PostAsJsonAsync($"/api/assignments/{assignment1!.Id}/accept",
            new AcceptAssignmentRequest(driver1Id));
        Assert.Equal(HttpStatusCode.OK, accept1.StatusCode);

        // Driver 2 tries to accept -> Conflict (409)
        var accept2 = await client.PostAsJsonAsync($"/api/assignments/{assignment2!.Id}/accept",
            new AcceptAssignmentRequest(driver2Id));
        Assert.Equal(HttpStatusCode.Conflict, accept2.StatusCode);
    }

    [Fact]
    public async Task PricingEstimate_ReturnsCalculatedFare()
    {
        using var factory = new MatchingPricingApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/pricing/estimate",
            new EstimatePriceRequest(10.7725m, 106.6980m, 10.7950m, 106.7218m));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<EstimatePriceResponse>();
        Assert.NotNull(result);
        Assert.True(result.Fare >= 15000m);
        Assert.True(result.DistanceKm > 0);
        Assert.Equal("VND", result.Currency);
    }

    [Fact]
    public async Task HealthCheck_Returns200Ok()
    {
        using var factory = new MatchingPricingApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/matching/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

