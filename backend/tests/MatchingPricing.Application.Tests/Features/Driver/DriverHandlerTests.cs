using BuildingBlocks.SharedKernel.Exceptions;
using MatchingPricing.Application.Features.Driver.Commands.CreateDriver;
using MatchingPricing.Application.Features.Driver.Commands.UpdateDriverLocation;
using MatchingPricing.Application.Features.Driver.Commands.UpdateDriverStatus;
using MatchingPricing.Application.Features.Driver.Queries.FindNearbyDrivers;
using MatchingPricing.Application.Features.Driver.Queries.GetDriverById;
using MatchingPricing.Application.Tests.Fakes;
using MatchingPricing.Domain.Entities;
using MatchingPricing.Domain.Enumerators;
using MatchingPricing.Domain.Errors;
using Xunit;

namespace MatchingPricing.Application.Tests.Features.Driver;

public class DriverHandlerTests
{
    private readonly FakeDriverRepository _repo = new();

    [Fact]
    public async Task CreateDriver_WithValidData_ShouldSucceed()
    {
        var handler = new CreateDriverHandler(_repo);
        var command = new CreateDriverCommand("Nguyen Driver", "0901111111", new DateOnly(1990, 1, 1));

        var result = await handler.HandleAsync(command);

        Assert.True(result.Id > 0);
        Assert.Equal("Nguyen Driver", result.FullName);
        Assert.Equal("0901111111", result.PhoneNumber);
        Assert.Equal(DriverStatus.offline.ToString(), result.Status);
        Assert.True(_repo.Saved);
    }

    [Fact]
    public async Task CreateDriver_WithDuplicatePhone_ShouldThrowConflict()
    {
        await _repo.AddAsync(Domain.Entities.Driver.Create("Driver 1", "0901111111"));

        var handler = new CreateDriverHandler(_repo);
        var command = new CreateDriverCommand("Driver 2", "0901111111");

        var ex = await Assert.ThrowsAsync<AppException>(() => handler.HandleAsync(command));
        Assert.Equal(MatchingPricingErrors.DriverAlreadyExists.Code, ex.Error.Code);
    }

    [Fact]
    public async Task UpdateDriverLocation_WhenValid_ShouldUpdateCoordinates()
    {
        var driver = Domain.Entities.Driver.Create("Driver", "0901111111");
        await _repo.AddAsync(driver);

        var handler = new UpdateDriverLocationHandler(_repo);
        var command = new UpdateDriverLocationCommand(driver.Id, 10.7725m, 106.6980m);

        var result = await handler.HandleAsync(command);

        Assert.Equal(10.7725m, result.Latitude);
        Assert.Equal(106.6980m, result.Longitude);
        Assert.Contains("POINT", result.Location);
        Assert.True(_repo.Saved);
    }

    [Fact]
    public async Task UpdateDriverStatus_WhenValid_ShouldUpdateStatus()
    {
        var driver = Domain.Entities.Driver.Create("Driver", "0901111111");
        await _repo.AddAsync(driver);

        var handler = new UpdateDriverStatusHandler(_repo);
        var command = new UpdateDriverStatusCommand(driver.Id, DriverStatus.available);

        var result = await handler.HandleAsync(command);

        Assert.Equal(DriverStatus.available.ToString(), result.Status);
    }

    [Fact]
    public async Task GetDriverById_WhenFound_ShouldReturnDriver()
    {
        var driver = Domain.Entities.Driver.Create("Driver", "0901111111");
        await _repo.AddAsync(driver);

        var handler = new GetDriverByIdHandler(_repo);
        var result = await handler.HandleAsync(new GetDriverByIdQuery(driver.Id));

        Assert.Equal(driver.Id, result.Id);
        Assert.Equal("Driver", result.FullName);
    }

    [Fact]
    public async Task FindNearbyDrivers_ShouldReturnAvailableDrivers()
    {
        var driver1 = Domain.Entities.Driver.Create("Driver Online", "0901111111", status: DriverStatus.available);
        driver1.UpdateLocation(10.7725m, 106.6980m);
        await _repo.AddAsync(driver1);

        var driver2 = Domain.Entities.Driver.Create("Driver Busy", "0902222222", status: DriverStatus.busy);
        driver2.UpdateLocation(10.7730m, 106.6990m);
        await _repo.AddAsync(driver2);

        var handler = new FindNearbyDriversHandler(_repo);
        var result = await handler.HandleAsync(new FindNearbyDriversQuery(10.7720m, 106.6970m));

        Assert.Single(result.Drivers);
        Assert.Equal(driver1.Id, result.Drivers[0].Driver.Id);
    }
}

