using Microsoft.EntityFrameworkCore;
using MiniBBS.DB;
using MiniBBS.Service;

namespace MiniBBS.Tests;

public class PostServiceTests
{
    private ForumDbContext GetContext()
    {
        var options = new DbContextOptionsBuilder<ForumDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ForumDbContext(options);
    }

    [Fact]
    public async Task CanCreatePost()
    {
        using var context = GetContext();
        context.Forums.Add(new Forum { ForumID = 1, ForumName = "Test", Description = "" });
        context.Users.Add(new User { Id = 1, UserName = "u" });
        await context.SaveChangesAsync();
        var service = new PostService(context);
        var post = await service.CreatePostAsync(new Post
        {
            Title = "t",
            Content = "c",
            ForumID = 1,
            UserID = 1,
            PostedTime = DateTime.UtcNow
        });
        Assert.NotEqual(0, post.PostID);

        var fetched = await service.GetPostByIdAsync(post.PostID);
        Assert.Equal("t", fetched.Title);
    }
}
