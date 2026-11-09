using BuildingBlocks.SharedKernel.Exceptions;
using Trip.Application.Features.Rider.Commands.CreateRider;
using Trip.Application.Features.Rider.Commands.UpdateRiderProfile;
using Trip.Application.Features.Rider.Queries.GetRiderById;
using Trip.Application.Features.Rider.Queries.GetRiders;
using Trip.Application.Tests.Fakes;
using Trip.Domain.Entities;
using Trip.Domain.Errors;
using Xunit;

namespace Trip.Application.Tests.Features.Rider;

public class RiderHandlerTests
{
    private readonly FakeRiderRepository _repo = new();

    [Fact]
    public async Task CreateRider_WithValidData_ShouldSucceed()
    {
        var handler = new CreateRiderHandler(_repo);
        var command = new CreateRiderCommand("Nguyen Van A", "0912345678", new DateOnly(1995, 1, 1));

        var result = await handler.HandleAsync(command);

        Assert.True(result.Id > 0);
        Assert.Equal("Nguyen Van A", result.FullName);
        Assert.Equal("0912345678", result.PhoneNumber);
        Assert.Equal(new DateOnly(1995, 1, 1), result.DateOfBirth);
        Assert.True(_repo.Saved);
    }

    [Fact]
    public async Task CreateRider_WithExistingPhoneNumber_ShouldThrowConflict()
    {
        var existing = Domain.Entities.Rider.Create("Existing Rider", "0912345678");
        await _repo.AddAsync(existing);

        var handler = new CreateRiderHandler(_repo);
        var command = new CreateRiderCommand("New Rider", "0912345678");

        var ex = await Assert.ThrowsAsync<AppException>(() => handler.HandleAsync(command));
        Assert.Equal(TripErrors.RiderAlreadyExists.Code, ex.Error.Code);
    }

    [Fact]
    public async Task UpdateRiderProfile_WithValidData_ShouldSucceed()
    {
        var rider = Domain.Entities.Rider.Create("Old Name", "0912345678");
        await _repo.AddAsync(rider);

        var handler = new UpdateRiderProfileHandler(_repo);
        var command = new UpdateRiderProfileCommand(rider.Id, "New Name", "0999999999", new DateOnly(2000, 2, 2));

        var result = await handler.HandleAsync(command);

        Assert.Equal("New Name", result.FullName);
        Assert.Equal("0999999999", result.PhoneNumber);
        Assert.Equal(new DateOnly(2000, 2, 2), result.DateOfBirth);
        Assert.True(_repo.Saved);
    }

    [Fact]
    public async Task UpdateRiderProfile_WhenRiderNotFound_ShouldThrowNotFound()
    {
        var handler = new UpdateRiderProfileHandler(_repo);
        var command = new UpdateRiderProfileCommand(999, "New Name", "0912345678");

        var ex = await Assert.ThrowsAsync<AppException>(() => handler.HandleAsync(command));
        Assert.Equal(TripErrors.RiderNotFound.Code, ex.Error.Code);
    }

    [Fact]
    public async Task UpdateRiderProfile_WithExistingPhoneOfAnotherRider_ShouldThrowConflict()
    {
        var rider1 = Domain.Entities.Rider.Create("Rider One", "0911111111");
        var rider2 = Domain.Entities.Rider.Create("Rider Two", "0922222222");
        await _repo.AddAsync(rider1);
        await _repo.AddAsync(rider2);

        var handler = new UpdateRiderProfileHandler(_repo);
        var command = new UpdateRiderProfileCommand(rider1.Id, "Rider One Updated", "0922222222");

        var ex = await Assert.ThrowsAsync<AppException>(() => handler.HandleAsync(command));
        Assert.Equal(TripErrors.RiderAlreadyExists.Code, ex.Error.Code);
    }

    [Fact]
    public async Task GetRiderById_WhenExists_ShouldReturnResult()
    {
        var rider = Domain.Entities.Rider.Create("Nguyen Van A", "0912345678");
        await _repo.AddAsync(rider);

        var handler = new GetRiderByIdHandler(_repo);
        var result = await handler.HandleAsync(new GetRiderByIdQuery(rider.Id));

        Assert.Equal(rider.Id, result.Id);
        Assert.Equal("Nguyen Van A", result.FullName);
    }

    [Fact]
    public async Task GetRiderById_WhenNotFound_ShouldThrowNotFound()
    {
        var handler = new GetRiderByIdHandler(_repo);
        var ex = await Assert.ThrowsAsync<AppException>(() =>
            handler.HandleAsync(new GetRiderByIdQuery(999)));

        Assert.Equal(TripErrors.RiderNotFound.Code, ex.Error.Code);
    }

    [Fact]
    public async Task GetRiders_ShouldReturnPagedResults()
    {
        await _repo.AddAsync(Domain.Entities.Rider.Create("Rider 1", "0900000001"));
        await _repo.AddAsync(Domain.Entities.Rider.Create("Rider 2", "0900000002"));

        var handler = new GetRidersHandler(_repo);
        var result = await handler.HandleAsync(new GetRidersQuery(1, 10));

        Assert.Equal(2, result.Items.Count);
    }
}

