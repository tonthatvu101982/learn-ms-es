using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers
{
    public class EventHandler : IEventHandler
    {
        private readonly IPostRepository _postRepository;

        private readonly ICommentRepository _commentRepository;

        public EventHandler(IPostRepository postRepository, ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
        }

       
        public async Task On(PostCreatedEvent @event)
        {
            var postEntity = new PostEntity
            {
                PostId = @event.Id, 
                Author =@event.Author,
                DatePosted = @event.DatePosted,
                Message = @event.Message
            };

            await _postRepository.CreateAsync(postEntity);
        }

        public async Task On(PostLikedEvent @event)
        {
            var post = await _postRepository.GetByIdAsync(@event.Id);
            if (post == null) return;
            post.Likes++;
            await _postRepository.UpdateAsync(post);
        }

        public async Task On(PostRemoveEvent @event)
        {
           await _postRepository.DeleteAsync(@event.Id);
        }

        public async Task On(MessageUpdatedEvent @event)
        {
            var post = await _postRepository.GetByIdAsync(@event.Id);
            if (post == null) return;
            post.Message = @event.Message;  
            await _postRepository.UpdateAsync(post);
        }

        public async Task On(CommentAddedEvent @event)
        {
            var comment = new CommentEntity
            {
                CommentId = @event.CommentId,
                CommentDate = @event.CommentDate,
                Username = @event.Username,
                PostId = @event.Id,
                Comment = @event.Comment,
                Edited = false
            };
            await _commentRepository.CreateAsync(comment);
        }

        public async Task On(CommentRemoveEvent @event)
        {
            await _commentRepository.DeleteAsync(@event.CommentId);
        }

        public async Task On(CommentUpdatedEvent @event)
        {
            var comment = await _commentRepository.GetByIdAsync(@event.CommentId);
            if (comment == null) return;
            comment.Comment = @event.Comment;
            comment.Edited = true;
            comment.CommentDate = @event.EditDate;
            await _commentRepository.UpdateAsync(comment);
        }
    }
}
