using System.Net;
using System.Net.Http.Json;
using Trip.Integration.Tests.Infrastructure;
using Trip.Presentation.Contracts.Requests;
using Trip.Presentation.Contracts.Responses;
using Xunit;

namespace Trip.Integration.Tests.Controllers;

public sealed class RidersControllerTests
{
    [Fact]
    public async Task CreateRider_WithValidData_Returns201Created()
    {
        using var factory = new TripApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var request = new CreateRiderRequest(
            "Nguyen Van Test",
            "0911000001",
            new DateOnly(1990, 5, 15)
        );

        var response = await client.PostAsJsonAsync("/api/riders", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<RiderResponse>();
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("Nguyen Van Test", result.FullName);
        Assert.Equal("0911000001", result.PhoneNumber);
        Assert.Equal(new DateOnly(1990, 5, 15), result.DateOfBirth);
    }

    [Fact]
    public async Task CreateRider_WithDuplicatePhone_Returns409Conflict()
    {
        using var factory = new TripApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var request1 = new CreateRiderRequest("Rider 1", "0911000002");
        await client.PostAsJsonAsync("/api/riders", request1);

        var request2 = new CreateRiderRequest("Rider 2", "0911000002");
        var response = await client.PostAsJsonAsync("/api/riders", request2);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task GetRiderById_WhenExists_Returns200Ok()
    {
        using var factory = new TripApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var request = new CreateRiderRequest("Rider Get", "0911000003");
        var createResponse = await client.PostAsJsonAsync("/api/riders", request);
        var created = await createResponse.Content.ReadFromJsonAsync<RiderResponse>();

        var response = await client.GetAsync($"/api/riders/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<RiderResponse>();
        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
    }

    [Fact]
    public async Task GetRiderById_WhenNotFound_Returns404NotFound()
    {
        using var factory = new TripApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var response = await client.GetAsync("/api/riders/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateRiderProfile_WhenValid_Returns200Ok()
    {
        using var factory = new TripApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var request = new CreateRiderRequest("Before Update", "0911000004");
        var createResponse = await client.PostAsJsonAsync("/api/riders", request);
        var created = await createResponse.Content.ReadFromJsonAsync<RiderResponse>();

        var updateRequest = new UpdateRiderProfileRequest("After Update", "0911000004", new DateOnly(1992, 2, 2));
        var response = await client.PutAsJsonAsync($"/api/riders/{created!.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<RiderResponse>();
        Assert.NotNull(result);
        Assert.Equal("After Update", result.FullName);
    }
}
