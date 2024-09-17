using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly DatabaseContextFactory _databaseContextFactory;
        public CommentRepository(DatabaseContextFactory databaseContextFactory)
        {
            _databaseContextFactory = databaseContextFactory;
        }
        public async Task CreateAsync(CommentEntity entity)
        {
            using var dbContext = _databaseContextFactory.CreateDbContext();
            dbContext.Add(entity);
            _ = await dbContext.SaveChangesAsync();

        }

        public async Task DeleteAsync(Guid commentId)
        {
            using var dbContext = _databaseContextFactory.CreateDbContext();
            var comment = await GetByIdAsync(commentId);
            if (comment == null)
            {
                return;
            }
            dbContext.Comments.Remove(comment);
            await dbContext.SaveChangesAsync();
        }

        public async Task<CommentEntity> GetByIdAsync(Guid commentId)
        {
            using var dbContext = _databaseContextFactory.CreateDbContext();
            return await dbContext.Comments.FirstOrDefaultAsync(x=>x.CommentId == commentId);
        }

        public async Task UpdateAsync(CommentEntity entity)
        {
            using var dbContext = _databaseContextFactory.CreateDbContext();

            dbContext.Comments.Update(entity);
            _= await dbContext.SaveChangesAsync();  
        }
    }
}
