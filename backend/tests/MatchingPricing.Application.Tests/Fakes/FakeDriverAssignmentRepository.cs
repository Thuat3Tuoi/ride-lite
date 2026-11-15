using MatchingPricing.Application.Abstractions.Data;
using MatchingPricing.Domain.Entities;
using MatchingPricing.Domain.Enumerators;

namespace MatchingPricing.Application.Tests.Fakes;

public sealed class FakeDriverAssignmentRepository : IDriverAssignmentRepository
{
    private long _nextId = 1;
    public List<DriverAssignment> Assignments { get; } = [];
    public bool Saved { get; private set; }

    public Task<DriverAssignment?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Assignments.FirstOrDefault(a => a.Id == id));
    }

    public Task<IReadOnlyList<DriverAssignment>> GetByTripIdAsync(long tripId, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<DriverAssignment> result = Assignments.Where(a => a.TripId == tripId).ToList();
        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<DriverAssignment>> GetByDriverIdAsync(long driverId, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<DriverAssignment> result = Assignments.Where(a => a.DriverId == driverId).ToList();
        return Task.FromResult(result);
    }

    public Task<DriverAssignment?> GetAcceptedAssignmentForTripAsync(long tripId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Assignments.FirstOrDefault(a => a.TripId == tripId && a.Status == AssignmentStatus.ACCEPTED));
    }

    public Task AddAsync(DriverAssignment assignment, CancellationToken cancellationToken = default)
    {
        if (assignment.Id == 0)
        {
            typeof(DriverAssignment).GetProperty("Id")?.SetValue(assignment, _nextId++);
        }

        Assignments.Add(assignment);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        Saved = true;
        return Task.CompletedTask;
    }
}

