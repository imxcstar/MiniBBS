using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiniBBS.DB;
using MiniBBS.Service;
using Xunit;

namespace MiniBBS.Tests.ServiceTests
{
    public class CommentServiceTests
    {
        private ForumDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ForumDbContext>()
                .UseInMemoryDatabase(System.Guid.NewGuid().ToString())
                .Options;
            return new ForumDbContext(options);
        }

        [Fact]
        public async Task AddCommentAsync_AddsComment()
        {
            using var context = CreateContext();
            context.Users.Add(new User { Id = 1, UserName = "u" });
            context.Forums.Add(new Forum { ForumID = 1, ForumName = "F" });
            context.Posts.Add(new Post { PostID = 1, Title = "t", Content = "c", ForumID = 1, UserID = 1 });
            await context.SaveChangesAsync();
            var service = new CommentService(context);
            var comment = new Comment { PostID = 1, UserID = 1, Content = "hi" };

            var result = await service.AddCommentAsync(comment);

            Assert.NotNull(result);
            Assert.Equal(1, await context.Comments.CountAsync());
        }
    }
}
