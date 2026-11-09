using MatchingPricing.Application.Abstractions.Data;
using MatchingPricing.Domain.Entities;
using MatchingPricing.Domain.Enumerators;
using MatchingPricing.Infrastructure.Data.DataContext;
using Microsoft.EntityFrameworkCore;

namespace MatchingPricing.Infrastructure.Data.Repositories;

public sealed class DriverAssignmentRepository : IDriverAssignmentRepository
{
    private readonly ApplicationDbContext _context;

    public DriverAssignmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DriverAssignment?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await _context.DriverAssignments
            .Include(a => a.Driver)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<DriverAssignment>> GetByTripIdAsync(
        long tripId,
        CancellationToken cancellationToken = default)
    {
        return await _context.DriverAssignments
            .Include(a => a.Driver)
            .Where(a => a.TripId == tripId)
            .OrderByDescending(a => a.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DriverAssignment>> GetByDriverIdAsync(
        long driverId,
        CancellationToken cancellationToken = default)
    {
        return await _context.DriverAssignments
            .Include(a => a.Driver)
            .Where(a => a.DriverId == driverId)
            .OrderByDescending(a => a.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<DriverAssignment?> GetAcceptedAssignmentForTripAsync(
        long tripId,
        CancellationToken cancellationToken = default)
    {
        return await _context.DriverAssignments
            .Include(a => a.Driver)
            .FirstOrDefaultAsync(a => a.TripId == tripId && a.Status == AssignmentStatus.ACCEPTED, cancellationToken);
    }

    public async Task AddAsync(
        DriverAssignment assignment,
        CancellationToken cancellationToken = default)
    {
        await _context.DriverAssignments.AddAsync(assignment, cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
