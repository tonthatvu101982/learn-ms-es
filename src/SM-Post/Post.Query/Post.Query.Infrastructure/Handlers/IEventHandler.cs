using CQRS.Core.Messages;
using Post.Common.Events;

namespace Post.Query.Infrastructure.Handlers;

public interface IEventHandler
{
    Task On(PostCreatedEvent @event);
    Task On(PostLikedEvent @event);
    Task On(PostRemoveEvent @event);

    Task On(MessageUpdatedEvent @event);

    Task On(CommentAddedEvent @event);

    Task On(CommentRemoveEvent @event);

    Task On(CommentUpdatedEvent @event);
}
