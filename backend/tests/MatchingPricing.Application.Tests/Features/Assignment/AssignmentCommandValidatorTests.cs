using MatchingPricing.Application.Features.Assignment.Commands.AcceptAssignment;
using MatchingPricing.Application.Features.Assignment.Commands.CreateAssignment;
using MatchingPricing.Application.Features.Assignment.Commands.RejectAssignment;
using MatchingPricing.Application.Features.Assignment.Queries.GetAssignmentsByTripId;
using MatchingPricing.Domain.Errors;
using Xunit;

namespace MatchingPricing.Application.Tests.Features.Assignment;

public sealed class AssignmentCommandValidatorTests
{
    private readonly CreateAssignmentCommandValidator _createValidator = new();
    private readonly AcceptAssignmentCommandValidator _acceptValidator = new();
    private readonly RejectAssignmentCommandValidator _rejectValidator = new();
    private readonly GetAssignmentsByTripIdQueryValidator _getByTripIdValidator = new();

    [Fact]
    public void CreateAssignment_WithValidData_ShouldBeValid()
    {
        var command = new CreateAssignmentCommand(100L, 1L, TimeSpan.FromSeconds(30));
        var result = _createValidator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateAssignment_WithNullDuration_ShouldBeValid()
    {
        var command = new CreateAssignmentCommand(100L, 1L, null);
        var result = _createValidator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreateAssignment_WithInvalidTripId_ShouldHaveInvalidTripIdError(long tripId)
    {
        var command = new CreateAssignmentCommand(tripId, 1L);
        var result = _createValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(MatchingPricingErrors.InvalidTripId, result.Errors[0].CustomState);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreateAssignment_WithInvalidDriverId_ShouldHaveInvalidDriverIdError(long driverId)
    {
        var command = new CreateAssignmentCommand(100L, driverId);
        var result = _createValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(MatchingPricingErrors.InvalidDriverId, result.Errors[0].CustomState);
    }

    [Fact]
    public void CreateAssignment_WithNegativeDuration_ShouldHaveInvalidParametersError()
    {
        var command = new CreateAssignmentCommand(100L, 1L, TimeSpan.FromSeconds(-5));
        var result = _createValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(MatchingPricingErrors.InvalidParameters, result.Errors[0].CustomState);
    }

    [Fact]
    public void AcceptAssignment_WithValidData_ShouldBeValid()
    {
        var command = new AcceptAssignmentCommand(1L, 2L);
        var result = _acceptValidator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AcceptAssignment_WithInvalidAssignmentId_ShouldHaveInvalidAssignmentIdError(long assignmentId)
    {
        var command = new AcceptAssignmentCommand(assignmentId, 1L);
        var result = _acceptValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(MatchingPricingErrors.InvalidAssignmentId, result.Errors[0].CustomState);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AcceptAssignment_WithInvalidDriverId_ShouldHaveInvalidDriverIdError(long driverId)
    {
        var command = new AcceptAssignmentCommand(1L, driverId);
        var result = _acceptValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(MatchingPricingErrors.InvalidDriverId, result.Errors[0].CustomState);
    }

    [Fact]
    public void RejectAssignment_WithValidData_ShouldBeValid()
    {
        var command = new RejectAssignmentCommand(1L, 2L);
        var result = _rejectValidator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void RejectAssignment_WithInvalidAssignmentId_ShouldHaveInvalidAssignmentIdError(long assignmentId)
    {
        var command = new RejectAssignmentCommand(assignmentId, 1L);
        var result = _rejectValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(MatchingPricingErrors.InvalidAssignmentId, result.Errors[0].CustomState);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void RejectAssignment_WithInvalidDriverId_ShouldHaveInvalidDriverIdError(long driverId)
    {
        var command = new RejectAssignmentCommand(1L, driverId);
        var result = _rejectValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(MatchingPricingErrors.InvalidDriverId, result.Errors[0].CustomState);
    }

    [Fact]
    public void GetAssignmentsByTripId_WithValidTripId_ShouldBeValid()
    {
        var query = new GetAssignmentsByTripIdQuery(100L);
        var result = _getByTripIdValidator.Validate(query);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GetAssignmentsByTripId_WithInvalidTripId_ShouldHaveInvalidTripIdError(long tripId)
    {
        var query = new GetAssignmentsByTripIdQuery(tripId);
        var result = _getByTripIdValidator.Validate(query);
        Assert.False(result.IsValid);
        Assert.Equal(MatchingPricingErrors.InvalidTripId, result.Errors[0].CustomState);
    }
}

