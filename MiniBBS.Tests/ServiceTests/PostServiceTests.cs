using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiniBBS.DB;
using MiniBBS.Service;
using Xunit;

namespace MiniBBS.Tests.ServiceTests
{
    public class PostServiceTests
    {
        private ForumDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ForumDbContext>()
                .UseInMemoryDatabase(System.Guid.NewGuid().ToString())
                .Options;
            return new ForumDbContext(options);
        }

        [Fact]
        public async Task CreatePostAsync_AddsPost()
        {
            using var context = CreateContext();
            context.Forums.Add(new Forum { ForumID = 1, ForumName = "F" });
            context.Users.Add(new User { Id = 1, UserName = "user" });
            await context.SaveChangesAsync();
            var service = new PostService(context);
            var post = new Post { Title = "t", Content = "c", ForumID = 1, UserID = 1 };

            var result = await service.CreatePostAsync(post);

            Assert.Equal(1, await context.Posts.CountAsync());
            Assert.Equal("t", result.Title);
        }

        [Fact]
        public async Task GetPostsByForumIdAsync_ReturnsOnlyForumPosts()
        {
            using var context = CreateContext();
            context.Forums.AddRange(
                new Forum { ForumID = 1, ForumName = "F" },
                new Forum { ForumID = 2, ForumName = "F2" }
            );
            context.Users.Add(new User { Id = 1, UserName = "user" });
            context.Posts.AddRange(
                new Post { PostID = 1, Title = "a", Content = "c", ForumID = 1, UserID = 1 },
                new Post { PostID = 2, Title = "b", Content = "c", ForumID = 2, UserID = 1 }
            );
            await context.SaveChangesAsync();
            var service = new PostService(context);

            var result = await service.GetPostsByForumIdAsync(1);

            Assert.Single(result);
            Assert.Equal(1, result.First().ForumID);
        }
    }
}
