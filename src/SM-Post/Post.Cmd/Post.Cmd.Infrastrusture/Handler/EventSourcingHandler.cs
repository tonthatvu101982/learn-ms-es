using CQRS.Core.Domains;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastrusture.Handler;

public class EventSourcingHandler : IEventSourceHandler<PostAggregate>
{
    private readonly IEventStore _eventStore;

    public EventSourcingHandler(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task<PostAggregate> GetByIdAsync(Guid id)
    {
        var aggregate = new PostAggregate();
        var events = await _eventStore.GetEventsAsync(id);
        if (events == null || !events.Any()) return aggregate;
        aggregate.ReplayEvents(events);
        var latest = events.Select(x => x.Version).Max();
        aggregate.Verstion = latest;
        return aggregate;
    }

    public async Task SaveAsync(AggregateRoot aggregateRoot)
    {
       await _eventStore.SaveEventAsync(aggregateRoot.Id, aggregateRoot.GetUncommittedChanges, aggregateRoot.Verstion);
        aggregateRoot.MarkChangesAsCommitted();
    }
}
