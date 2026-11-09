using System.Net;
using System.Net.Http.Json;
using Trip.Domain.Enumerators;
using Trip.Integration.Tests.Infrastructure;
using Trip.Presentation.Contracts.Requests;
using Trip.Presentation.Contracts.Responses;
using Xunit;

namespace Trip.Integration.Tests.Controllers;

public sealed class TripsControllerTests
{
    private static async Task<long> CreateRiderAsync(HttpClient client, string phone)
    {
        var response = await client.PostAsJsonAsync("/api/riders", new CreateRiderRequest(
            "Test Rider",
            phone,
            new DateOnly(1993, 3, 3)
        ));
        var rider = await response.Content.ReadFromJsonAsync<RiderResponse>();
        return rider!.Id;
    }

    [Fact]
    public async Task CreateTrip_WithValidData_Returns201CreatedAndCalculatesFare()
    {
        using var factory = new TripApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var riderId = await CreateRiderAsync(client, "0922000001");
        var request = new CreateTripRequest(
            RiderId: riderId,
            PickupAddress: "Ben Thanh, District 1, HCMC",
            PickupLatitude: 10.7725m,
            PickupLongitude: 106.6980m,
            DropoffAddress: "Landmark 81, Binh Thanh, HCMC",
            DropoffLatitude: 10.7950m,
            DropoffLongitude: 106.7218m,
            ClientFare: 1000m // Client offers very low fare
        );

        var response = await client.PostAsJsonAsync("/api/trips", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<TripResponse>();
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(riderId, result.RiderId);
        Assert.Equal("SEARCHING_DRIVER", result.Status);
        // Persisted price MUST be authoritative from PricingService, not 1000m!
        Assert.True(result.Cost >= 15000m);
    }

    [Fact]
    public async Task CreateTrip_WithNonExistentRider_Returns404NotFound()
    {
        using var factory = new TripApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var request = new CreateTripRequest(
            RiderId: 999999,
            PickupAddress: "A",
            PickupLatitude: 10.77m,
            PickupLongitude: 106.69m,
            DropoffAddress: "B",
            DropoffLatitude: 10.79m,
            DropoffLongitude: 106.72m
        );

        var response = await client.PostAsJsonAsync("/api/trips", request);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task TripLifecycle_FullWorkflow_Succeeds()
    {
        using var factory = new TripApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var riderId = await CreateRiderAsync(client, "0922000002");

        // 1. Create Trip (SEARCHING_DRIVER)
        var createResponse = await client.PostAsJsonAsync("/api/trips", new CreateTripRequest(
            riderId, "Start", 10.77m, 106.69m, "End", 10.79m, 106.72m));
        var trip = await createResponse.Content.ReadFromJsonAsync<TripResponse>();
        Assert.NotNull(trip);
        var tripId = trip.Id;

        // 2. Accept Driver -> DRIVER_ACCEPTED
        var acceptResponse = await client.PostAsJsonAsync($"/api/trips/{tripId}/accept-driver",
            new AcceptTripDriverRequest(555L));
        Assert.Equal(HttpStatusCode.OK, acceptResponse.StatusCode);

        // 3. Driver Arrived -> DRIVER_ARRIVED
        var arrivedResponse = await client.PostAsJsonAsync($"/api/trips/{tripId}/driver-arrived", new { });
        Assert.Equal(HttpStatusCode.OK, arrivedResponse.StatusCode);

        // 4. Start Trip -> IN_PROGRESS
        var startResponse = await client.PostAsJsonAsync($"/api/trips/{tripId}/start", new { });
        Assert.Equal(HttpStatusCode.OK, startResponse.StatusCode);

        // 5. Complete Trip -> COMPLETED
        var completeResponse = await client.PostAsJsonAsync($"/api/trips/{tripId}/complete", new { });
        Assert.Equal(HttpStatusCode.OK, completeResponse.StatusCode);

        // 6. Verify final state via GET /api/trips/{id}
        var getResponse = await client.GetAsync($"/api/trips/{tripId}");
        var finalTrip = await getResponse.Content.ReadFromJsonAsync<TripResponse>();
        Assert.NotNull(finalTrip);
        Assert.Equal("COMPLETED", finalTrip.Status);
        Assert.Equal(555L, finalTrip.DriverId);
        Assert.NotNull(finalTrip.AcceptedAt);
        Assert.NotNull(finalTrip.DriverArrivedAt);
        Assert.NotNull(finalTrip.StartedAt);
        Assert.NotNull(finalTrip.CompletedAt);
    }

    [Fact]
    public async Task CancelTrip_WhenSearching_Returns200Ok()
    {
        using var factory = new TripApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var riderId = await CreateRiderAsync(client, "0922000003");
        var createResponse = await client.PostAsJsonAsync("/api/trips", new CreateTripRequest(
            riderId, "Start", 10.77m, 106.69m, "End", 10.79m, 106.72m));
        var trip = await createResponse.Content.ReadFromJsonAsync<TripResponse>();

        var cancelResponse = await client.PostAsJsonAsync($"/api/trips/{trip!.Id}/cancel",
            new CancelTripRequest(CancelledBy.RIDER, "Found another ride"));
        Assert.Equal(HttpStatusCode.OK, cancelResponse.StatusCode);

        var getResponse = await client.GetAsync($"/api/trips/{trip.Id}");
        var cancelledTrip = await getResponse.Content.ReadFromJsonAsync<TripResponse>();
        Assert.NotNull(cancelledTrip);
        Assert.Equal("CANCELLED", cancelledTrip.Status);
        Assert.Equal("RIDER", cancelledTrip.CancelledBy);
        Assert.Equal("Found another ride", cancelledTrip.CancelReason);
        Assert.NotNull(cancelledTrip.CancelledAt);
    }

    [Fact]
    public async Task MarkNoDriverFound_WhenSearching_Returns200Ok()
    {
        using var factory = new TripApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var riderId = await CreateRiderAsync(client, "0922000004");
        var createResponse = await client.PostAsJsonAsync("/api/trips", new CreateTripRequest(
            riderId, "Start", 10.77m, 106.69m, "End", 10.79m, 106.72m));
        var trip = await createResponse.Content.ReadFromJsonAsync<TripResponse>();

        var response = await client.PostAsJsonAsync($"/api/trips/{trip!.Id}/no-driver", new { });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var getResponse = await client.GetAsync($"/api/trips/{trip.Id}");
        var finalTrip = await getResponse.Content.ReadFromJsonAsync<TripResponse>();
        Assert.NotNull(finalTrip);
        Assert.Equal("NO_DRIVER_FOUND", finalTrip.Status);
    }

    [Fact]
    public async Task HealthCheck_Returns200Ok()
    {
        using var factory = new TripApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/trip/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
