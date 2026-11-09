using Trip.Application.Abstractions.Data;

namespace Trip.Application.Behaviors;

public static class TransactionBehavior
{
    public static Task<int> SaveChangesAsync(
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken = default) =>
        unitOfWork.SaveChangesAsync(cancellationToken);
}
