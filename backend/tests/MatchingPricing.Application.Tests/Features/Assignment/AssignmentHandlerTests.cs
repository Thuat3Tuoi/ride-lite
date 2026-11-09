using BuildingBlocks.SharedKernel.Exceptions;
using MatchingPricing.Application.Features.Assignment.Commands.AcceptAssignment;
using MatchingPricing.Application.Features.Assignment.Commands.CreateAssignment;
using MatchingPricing.Application.Features.Assignment.Commands.RejectAssignment;
using MatchingPricing.Application.Features.Assignment.Queries.GetAssignmentsByTripId;
using MatchingPricing.Application.Features.Pricing.Queries.EstimatePrice;
using MatchingPricing.Application.Tests.Fakes;
using MatchingPricing.Domain.Entities;
using MatchingPricing.Domain.Enumerators;
using MatchingPricing.Domain.Errors;
using Xunit;

namespace MatchingPricing.Application.Tests.Features.Assignment;

public class AssignmentHandlerTests
{
    private readonly FakeDriverAssignmentRepository _assignmentRepo = new();
    private readonly FakeDriverRepository _driverRepo = new();
    private readonly FakePricingCalculator _pricingCalculator = new();

    private async Task<Domain.Entities.Driver> SeedAvailableDriverAsync()
    {
        var driver = Domain.Entities.Driver.Create("Driver", "0901234567", status: DriverStatus.available);
        driver.UpdateLocation(10.77m, 106.69m);
        await _driverRepo.AddAsync(driver);
        return driver;
    }

    [Fact]
    public async Task CreateAssignment_WhenDriverAvailable_ShouldCreateOfferedAssignment()
    {
        var driver = await SeedAvailableDriverAsync();
        var handler = new CreateAssignmentHandler(_assignmentRepo, _driverRepo);
        var command = new CreateAssignmentCommand(100L, driver.Id, TimeSpan.FromSeconds(30));

        var result = await handler.HandleAsync(command);

        Assert.True(result.Id > 0);
        Assert.Equal(100L, result.TripId);
        Assert.Equal(driver.Id, result.DriverId);
        Assert.Equal(AssignmentStatus.OFFERED.ToString(), result.Status);
    }

    [Fact]
    public async Task CreateAssignment_WhenDriverNotAvailable_ShouldThrow()
    {
        var driver = Domain.Entities.Driver.Create("Driver", "0901234567", status: DriverStatus.offline);
        await _driverRepo.AddAsync(driver);

        var handler = new CreateAssignmentHandler(_assignmentRepo, _driverRepo);
        var command = new CreateAssignmentCommand(100L, driver.Id);

        var ex = await Assert.ThrowsAsync<AppException>(() => handler.HandleAsync(command));
        Assert.Equal(MatchingPricingErrors.DriverNotAvailable.Code, ex.Error.Code);
    }

    [Fact]
    public async Task AcceptAssignment_WhenValid_ShouldAcceptAndSetDriverBusy()
    {
        var driver = await SeedAvailableDriverAsync();
        var assignment = DriverAssignment.Create(100L, driver.Id, TimeSpan.FromMinutes(2));
        await _assignmentRepo.AddAsync(assignment);

        var handler = new AcceptAssignmentHandler(_assignmentRepo, _driverRepo);
        var result = await handler.HandleAsync(new AcceptAssignmentCommand(assignment.Id, driver.Id));

        Assert.Equal(AssignmentStatus.ACCEPTED.ToString(), result.Status);
        Assert.Equal(DriverStatus.busy, driver.Status);
    }

    [Fact]
    public async Task AcceptAssignment_WhenTripAlreadyAcceptedByAnother_ShouldThrowConflict()
    {
        var driver1 = await SeedAvailableDriverAsync();
        var driver2 = Domain.Entities.Driver.Create("Driver 2", "0909999999", status: DriverStatus.available);
        await _driverRepo.AddAsync(driver2);

        // Driver 1 accepted first
        var assignment1 = DriverAssignment.Create(100L, driver1.Id, TimeSpan.FromMinutes(2));
        assignment1.Accept();
        await _assignmentRepo.AddAsync(assignment1);

        // Driver 2 tries to accept later
        var assignment2 = DriverAssignment.Create(100L, driver2.Id, TimeSpan.FromMinutes(2));
        await _assignmentRepo.AddAsync(assignment2);

        var handler = new AcceptAssignmentHandler(_assignmentRepo, _driverRepo);
        var ex = await Assert.ThrowsAsync<AppException>(() =>
            handler.HandleAsync(new AcceptAssignmentCommand(assignment2.Id, driver2.Id)));

        Assert.Equal(MatchingPricingErrors.TripAlreadyAssigned.Code, ex.Error.Code);
    }

    [Fact]
    public async Task AcceptAssignment_WhenDriverMismatch_ShouldThrowForbidden()
    {
        var driver = await SeedAvailableDriverAsync();
        var assignment = DriverAssignment.Create(100L, driver.Id, TimeSpan.FromMinutes(2));
        await _assignmentRepo.AddAsync(assignment);

        var handler = new AcceptAssignmentHandler(_assignmentRepo, _driverRepo);
        var ex = await Assert.ThrowsAsync<AppException>(() =>
            handler.HandleAsync(new AcceptAssignmentCommand(assignment.Id, 999L)));

        Assert.Equal(MatchingPricingErrors.DriverMismatch.Code, ex.Error.Code);
    }

    [Fact]
    public async Task RejectAssignment_WhenValid_ShouldReject()
    {
        var driver = await SeedAvailableDriverAsync();
        var assignment = DriverAssignment.Create(100L, driver.Id, TimeSpan.FromMinutes(2));
        await _assignmentRepo.AddAsync(assignment);

        var handler = new RejectAssignmentHandler(_assignmentRepo);
        var result = await handler.HandleAsync(new RejectAssignmentCommand(assignment.Id, driver.Id));

        Assert.Equal(AssignmentStatus.REJECTED.ToString(), result.Status);
    }

    [Fact]
    public async Task EstimatePrice_ShouldReturnCalculatedFare()
    {
        _pricingCalculator.FixedFare = 60000m;
        var handler = new EstimatePriceHandler(_pricingCalculator);
        var query = new EstimatePriceQuery(10.77m, 106.69m, 10.79m, 106.72m);

        var result = await handler.HandleAsync(query);

        Assert.Equal(60000m, result.Fare);
        Assert.True(result.DistanceKm > 0);
        Assert.Equal("VND", result.Currency);
    }
}

