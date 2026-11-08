namespace Trip.Application.Abstractions;

public interface IEventBus
{
    Task PublishAsync(object domainEvent, CancellationToken cancellationToken = default);
}
