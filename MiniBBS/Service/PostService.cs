using Microsoft.EntityFrameworkCore;
using MiniBBS.DB;

namespace MiniBBS.Service
{
    public class PostService : IPostService
    {
        private readonly ForumDbContext _dbContext;

        public PostService(ForumDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Post>> GetPostsByForumIdAsync(int forumId)
        {
            return await _dbContext.Posts.Include(x => x.User).Include(x => x.Comments).Where(p => p.ForumID == forumId).ToListAsync();
        }

        public async Task<Post> GetPostByIdAsync(int postId)
        {
            return await _dbContext.Posts.Include(x => x.User).Include(x => x.Comments).FirstAsync(x => x.PostID == postId);
        }

        public async Task<Post> CreatePostAsync(Post post)
        {
            _dbContext.Posts.Add(post);
            await _dbContext.SaveChangesAsync();
            return post;
        }

        public async Task UpdatePostAsync(Post post)
        {
            _dbContext.Entry(post).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeletePostAsync(int postId)
        {
            var post = await _dbContext.Posts.FindAsync(postId);
            if (post != null)
            {
                _dbContext.Posts.Remove(post);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
