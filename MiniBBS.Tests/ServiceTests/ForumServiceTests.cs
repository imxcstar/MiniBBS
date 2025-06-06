using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiniBBS.DB;
using MiniBBS.Service;
using Xunit;

namespace MiniBBS.Tests.ServiceTests
{
    public class ForumServiceTests
    {
        private ForumDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ForumDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            return new ForumDbContext(options);
        }

        [Fact]
        public async Task CreateForumAsync_AddsForum()
        {
            using var context = CreateContext();
            var service = new ForumService(context);
            var forum = new Forum { ForumName = "Test", Description = "Desc" };

            var result = await service.CreateForumAsync(forum);

            Assert.NotNull(result);
            Assert.Equal(1, await context.Forums.CountAsync());
        }

        [Fact]
        public async Task DeleteForumAsync_RemovesForum()
        {
            using var context = CreateContext();
            context.Forums.Add(new Forum { ForumID = 1, ForumName = "A" });
            await context.SaveChangesAsync();
            var service = new ForumService(context);

            var removed = await service.DeleteForumAsync(1);

            Assert.True(removed);
            Assert.Empty(context.Forums);
        }

        [Fact]
        public async Task DeleteForumAsync_ReturnsFalse_WhenNotFound()
        {
            using var context = CreateContext();
            var service = new ForumService(context);

            var removed = await service.DeleteForumAsync(99);

            Assert.False(removed);
        }
    }
}
