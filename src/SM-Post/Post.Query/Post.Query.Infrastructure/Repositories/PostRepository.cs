using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;
using System.Net.NetworkInformation;

namespace Post.Query.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly DatabaseContextFactory _databaseContextFactory;

    public PostRepository(DatabaseContextFactory databaseContextFactory)
    {
        _databaseContextFactory = databaseContextFactory;
    }
    public async Task CreateAsync(PostEntity entity)
    {
        using var dbContext = _databaseContextFactory.CreateDbContext();
        dbContext.Add(entity);
        _ = await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid postId)
    {
        using var dbContext = _databaseContextFactory.CreateDbContext();
        var post = await GetByIdAsync(postId);
        if (post == null)
        {
            return;
        }

        dbContext.Remove(post);
        _ = await dbContext.SaveChangesAsync();

    }

    public async Task<PostEntity> GetByIdAsync(Guid postId)
    {
        using var dbContext = _databaseContextFactory.CreateDbContext();
        var postEntity =  await dbContext.Posts.Include(p => p.Comments).FirstOrDefaultAsync(x => x.PostId == postId);
        return postEntity;
    }

    public async Task<List<PostEntity>> ListAllAsync()
    {
        using var dbContext = _databaseContextFactory.CreateDbContext();
        return await dbContext.Posts.Include(p => p.Comments)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<PostEntity>> ListByAuthorsAsync(string author)
    {
        using var dbContext = _databaseContextFactory.CreateDbContext();
        return await dbContext.Posts.Include(p => p.Comments)
            .AsNoTracking()
            .Where(x => x.Author.Contains(author))
            .ToListAsync();
    }

    public async Task<List<PostEntity>> ListWithCommentsAsync()
    {
        using var dbContext = _databaseContextFactory.CreateDbContext();
        return await dbContext.Posts.Include(p => p.Comments)
            .AsNoTracking()
            .Where(x => x.Comments != null && x.Comments.Any())
            .ToListAsync();
    }

    public async Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes)
    {
        using var dbContext = _databaseContextFactory.CreateDbContext();
        return await dbContext.Posts.Include(p => p.Comments)
            .AsNoTracking()
            .Where(x => x.Likes >= numberOfLikes)
            .ToListAsync();
    }

    public async Task UpdateAsync(PostEntity entity)
    {
        using var dbContext = _databaseContextFactory.CreateDbContext();
        dbContext.Posts.Update(entity);
        _ = await dbContext.SaveChangesAsync();
    }
}
