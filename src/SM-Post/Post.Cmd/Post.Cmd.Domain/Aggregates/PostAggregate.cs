using CQRS.Core.Domains;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregates;

public class PostAggregate : AggregateRoot
{
    private bool _active;

    private string _author;

    private readonly IDictionary<Guid, Tuple<string, string>> _comments = new Dictionary<Guid, Tuple<string, string>>();

    public bool Active
    {
        get => _active; set => _active = value;
    }
    public PostAggregate()
    {

    }

    public PostAggregate(Guid id, string author, string message)
    {
        RaiseEvent(new PostCreatedEvent
        {
            Id = id,
            Author = author,
            Message = message,
            DatePosted = DateTime.Now
        });

    }

    public void Apply(PostCreatedEvent @event)
    {
        _id = @event.Id;
        _author = @event.Author;
        _active = true;

    }

    public void EditMessage(string message)
    {
        if (!_active)
        {
            throw new InvalidOperationException("You can not edit the message of an inactive post!");
        }
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new InvalidOperationException($"The value of {nameof(message)} could be null or empty. Please provide a valid {nameof(message)}!");
        }

        RaiseEvent(new MessageUpdatedEvent { Id = _id, Message = message });

    }

    public void Apply(MessageUpdatedEvent @event)
    {
        _id = @event.Id;
    }

    public void LikePost()
    {
        if (!_active)
        {
            throw new InvalidOperationException("You can not like an inactive post!");
        }
        RaiseEvent(new PostLikedEvent
        {
            Id = _id
        });
    }

    public void Apply(PostLikedEvent @event)
    {
        _id = @event.Id;
    }

    public void AddComment(string comment, string username)
    {
        if (!_active)
        {
            throw new InvalidOperationException("You can not add comment to an inactive post!");
        }

        if (string.IsNullOrWhiteSpace(comment))
        {
            throw new InvalidOperationException($"The value of {nameof(comment)} could be null or empty. Please provide a valid {nameof(comment)}!");
        }
        RaiseEvent(new CommentAddedEvent
        {
            Id = _id,
            CommentId = Guid.NewGuid(),
            Comment = comment,
            Username = username,
            CommentDate = DateTime.Now
        });
    }

    public void Apply(CommentAddedEvent @event)
    {
        _id = @event.Id;
        _comments.Add(@event.CommentId, new Tuple<string, string>(@event.Comment, @event.Username));
    }

    public void EditComment(Guid commentId, string comment, string username)
    {
        if (!_active)
        {
            throw new InvalidOperationException("You can not edit comment to an inactive post!");
        }

        if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException("You are not allowed to edit a comment that was made by another user!");
        }
        RaiseEvent(new CommentUpdatedEvent
        {
            Id = _id,
            CommentId = commentId,
            Username = username,
            Comment = comment,
            EditDate = DateTime.Now
        });
    }

    public void Apply(CommentUpdatedEvent @event)
    {
        _id = @event.Id;
        _comments[@event.Id] = new Tuple<string, string>(@event.Comment, @event.Username);
    }

    public void RemoveComment(Guid commentId, string username)
    {
        if (!_active)
        {
            throw new InvalidOperationException("You can not remove a comment to an inactive post!");
        }
        if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException("You are not allowed to remove a comment that was made by another user!");
        }

        RaiseEvent(new CommentRemoveEvent { Id = _id, CommentId = commentId });
    }

    public void Apply(CommentRemoveEvent @event)
    {
        _id = @event.Id;
        _comments.Remove(@event.CommentId);
    }

    public void DeletePost(string username)
    {
        if (!_active)
        {
            throw new InvalidOperationException("The post has already been removed!");
        }

        if (!_author.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException("You are not allowed to delete a post that was made  by somebody else!");
        }

        RaiseEvent(new PostRemoveEvent
        {
            Id = _id,
        });
    }

    public void Apply(PostRemoveEvent @event)
    {
        _id = @event.Id;
        _active = false;
    }
}
