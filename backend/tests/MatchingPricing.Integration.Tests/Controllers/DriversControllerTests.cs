using System.Net;
using System.Net.Http.Json;
using MatchingPricing.Domain.Enumerators;
using MatchingPricing.Integration.Tests.Infrastructure;
using MatchingPricing.Presentation.Contracts.Requests;
using MatchingPricing.Presentation.Contracts.Responses;
using Xunit;

namespace MatchingPricing.Integration.Tests.Controllers;

public sealed class DriversControllerTests
{
    [Fact]
    public async Task CreateDriver_WithValidData_Returns201Created()
    {
        using var factory = new MatchingPricingApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var request = new CreateDriverRequest(
            "Le Van Driver",
            "0909000001",
            new DateOnly(1989, 9, 9),
            DriverStatus.offline
        );

        var response = await client.PostAsJsonAsync("/api/drivers", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<DriverResponse>();
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("Le Van Driver", result.FullName);
        Assert.Equal("0909000001", result.PhoneNumber);
        Assert.Equal("offline", result.Status);
    }

    [Fact]
    public async Task CreateDriver_WithDuplicatePhone_Returns409Conflict()
    {
        using var factory = new MatchingPricingApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var request1 = new CreateDriverRequest("D1", "0909000002");
        await client.PostAsJsonAsync("/api/drivers", request1);

        var request2 = new CreateDriverRequest("D2", "0909000002");
        var response = await client.PostAsJsonAsync("/api/drivers", request2);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task UpdateLocationAndStatus_ThenFindNearby_ReturnsDriver()
    {
        using var factory = new MatchingPricingApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        // 1. Create driver
        var createResponse = await client.PostAsJsonAsync("/api/drivers",
            new CreateDriverRequest("Online Driver", "0909000003"));
        var driver = await createResponse.Content.ReadFromJsonAsync<DriverResponse>();

        // 2. Go online (available)
        var statusResponse = await client.PostAsJsonAsync($"/api/drivers/{driver!.Id}/status",
            new UpdateDriverStatusRequest(DriverStatus.available));
        Assert.Equal(HttpStatusCode.OK, statusResponse.StatusCode);

        // 3. Update location
        var locationResponse = await client.PostAsJsonAsync($"/api/drivers/{driver.Id}/location",
            new UpdateDriverLocationRequest(10.7725m, 106.6980m));
        Assert.Equal(HttpStatusCode.OK, locationResponse.StatusCode);

        // 4. Query nearby
        var nearbyResponse = await client.GetAsync("/api/drivers/nearby?pickupLatitude=10.7730&pickupLongitude=106.6990&radiusKm=5");
        Assert.Equal(HttpStatusCode.OK, nearbyResponse.StatusCode);

        var nearbyList = await nearbyResponse.Content.ReadFromJsonAsync<IReadOnlyList<NearbyDriverResponse>>();
        Assert.NotNull(nearbyList);
        Assert.Single(nearbyList);
        Assert.Equal(driver.Id, nearbyList[0].Driver.Id);
    }
}

