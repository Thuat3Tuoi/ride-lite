namespace Trip.Application.Abstractions.Messaging;

public interface IEventBus
{
    Task PublishAsync(object domainEvent, CancellationToken cancellationToken = default);
}
