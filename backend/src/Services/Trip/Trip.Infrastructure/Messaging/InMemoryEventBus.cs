using System.Collections.Concurrent;
using Trip.Application.Abstractions;

namespace Trip.Infrastructure.Messaging;

public sealed class InMemoryEventBus : IEventBus
{
    private readonly ConcurrentQueue<object> _publishedEvents = new();

    public IReadOnlyCollection<object> PublishedEvents => _publishedEvents.ToArray();

    public Task PublishAsync(object domainEvent, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        _publishedEvents.Enqueue(domainEvent);
        return Task.CompletedTask;
    }
}
