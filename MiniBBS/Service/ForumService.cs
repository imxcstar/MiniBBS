using Microsoft.EntityFrameworkCore;
using MiniBBS.DB;

namespace MiniBBS.Service
{
    public class ForumService : IForumService
    {
        private readonly ForumDbContext _context;

        public ForumService(ForumDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Forum>> GetAllForumsAsync()
        {
            return await _context.Forums.ToListAsync();
        }

        public async Task<Forum> GetForumByIdAsync(int id)
        {
            return await _context.Forums.FindAsync(id);
        }

        public async Task<Forum> CreateForumAsync(Forum forum)
        {
            _context.Forums.Add(forum);
            await _context.SaveChangesAsync();
            return forum;
        }

        public async Task<Forum> UpdateForumAsync(int id, Forum forum)
        {
            var existingForum = await _context.Forums.FindAsync(id);
            if (existingForum == null) return null;

            existingForum.ForumName = forum.ForumName;
            existingForum.Description = forum.Description;

            await _context.SaveChangesAsync();
            return existingForum;
        }

        public async Task<bool> DeleteForumAsync(int id)
        {
            var forum = await _context.Forums.FindAsync(id);
            if (forum == null) return false;

            _context.Forums.Remove(forum);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
