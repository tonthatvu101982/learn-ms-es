using Post.Query.Domain.Entities;

namespace Post.Query.Domain.Repositories;

public interface IPostRepository
{
    Task CreateAsync(PostEntity entity);

    Task UpdateAsync(PostEntity entity);
    Task DeleteAsync(Guid postId);

    Task<PostEntity> GetByIdAsync(Guid postId);

    Task<List<PostEntity>> ListAllAsync();

    Task<List<PostEntity>> ListByAuthorsAsync(string author);

    Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes);

    Task<List<PostEntity>> ListWithCommentsAsync();
}
