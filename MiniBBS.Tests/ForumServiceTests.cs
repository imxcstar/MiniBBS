using Microsoft.EntityFrameworkCore;
using MiniBBS.DB;
using MiniBBS.Service;

namespace MiniBBS.Tests;

public class ForumServiceTests
{
    private ForumDbContext GetContext()
    {
        var options = new DbContextOptionsBuilder<ForumDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ForumDbContext(options);
    }

    [Fact]
    public async Task CanCreateAndRetrieveForum()
    {
        using var context = GetContext();
        var service = new ForumService(context);
        var forum = await service.CreateForumAsync(new Forum { ForumName = "Test", Description = "Desc" });

        var fetched = await service.GetForumByIdAsync(forum.ForumID);
        Assert.NotNull(fetched);
        Assert.Equal("Test", fetched.ForumName);
    }

    [Fact]
    public async Task CanUpdateAndDeleteForum()
    {
        using var context = GetContext();
        var service = new ForumService(context);
        var forum = await service.CreateForumAsync(new Forum { ForumName = "Old", Description = "d" });
        var updated = await service.UpdateForumAsync(forum.ForumID, new Forum { ForumName = "New", Description = "d2" });
        Assert.Equal("New", updated?.ForumName);

        var result = await service.DeleteForumAsync(forum.ForumID);
        Assert.True(result);
        Assert.Empty(await service.GetAllForumsAsync());
    }
}
