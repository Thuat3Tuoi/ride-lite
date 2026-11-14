using MatchingPricing.Domain.Entities;
using MatchingPricing.Domain.Enumerators;
using Xunit;

namespace MatchingPricing.Domain.Tests;

public class DriverAssignmentTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateOfferedAssignment()
    {
        var assignment = DriverAssignment.Create(100L, 5L, TimeSpan.FromSeconds(45));

        Assert.Equal(100L, assignment.TripId);
        Assert.Equal(5L, assignment.DriverId);
        Assert.Equal(AssignmentStatus.OFFERED, assignment.Status);
        Assert.Null(assignment.RespondedAt);
        Assert.True(assignment.ExpiresAt > assignment.OfferedAt);
    }

    [Theory]
    [InlineData(0, 5)]
    [InlineData(-1, 5)]
    [InlineData(100, 0)]
    [InlineData(100, -1)]
    public void Create_WithInvalidIds_ShouldThrow(long tripId, long driverId)
    {
        Assert.Throws<ArgumentException>(() =>
            DriverAssignment.Create(tripId, driverId));
    }

    [Fact]
    public void Accept_WhenOffered_ShouldTransitionToAccepted()
    {
        var assignment = DriverAssignment.Create(100L, 5L, TimeSpan.FromMinutes(5));

        assignment.Accept();

        Assert.Equal(AssignmentStatus.ACCEPTED, assignment.Status);
        Assert.NotNull(assignment.RespondedAt);
    }

    [Fact]
    public void Reject_WhenOffered_ShouldTransitionToRejected()
    {
        var assignment = DriverAssignment.Create(100L, 5L, TimeSpan.FromMinutes(5));

        assignment.Reject();

        Assert.Equal(AssignmentStatus.REJECTED, assignment.Status);
        Assert.NotNull(assignment.RespondedAt);
    }

    [Fact]
    public void Expire_WhenOffered_ShouldTransitionToExpired()
    {
        var assignment = DriverAssignment.Create(100L, 5L, TimeSpan.FromMinutes(5));

        assignment.Expire();

        Assert.Equal(AssignmentStatus.EXPIRED, assignment.Status);
    }

    [Fact]
    public void Cancel_WhenOffered_ShouldTransitionToCancelled()
    {
        var assignment = DriverAssignment.Create(100L, 5L, TimeSpan.FromMinutes(5));

        assignment.Cancel();

        Assert.Equal(AssignmentStatus.CANCELLED, assignment.Status);
    }

    [Fact]
    public void Accept_WhenAlreadyAccepted_ShouldThrow()
    {
        var assignment = DriverAssignment.Create(100L, 5L, TimeSpan.FromMinutes(5));
        assignment.Accept();

        Assert.Throws<InvalidOperationException>(() => assignment.Accept());
    }

    [Fact]
    public void Accept_WhenExpiredDeadline_ShouldMarkExpiredAndThrow()
    {
        // Past deadline
        var assignment = DriverAssignment.Create(100L, 5L, TimeSpan.FromMilliseconds(1));
        Thread.Sleep(10); // Wait for expiration

        Assert.Throws<InvalidOperationException>(() => assignment.Accept());
        Assert.Equal(AssignmentStatus.EXPIRED, assignment.Status);
    }
}

